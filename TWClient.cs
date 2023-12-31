/*   This file is part of TWLib.
 *
 *    TWLib is free software: you can redistribute it and/or modify
 *    it under the terms of the GNU General Public License as published by
 *    the Free Software Foundation, either version 3 of the License, or
 *    (at your option) any later version.

 *    TWLib is distributed in the hope that it will be useful,
 *    but WITHOUT ANY WARRANTY; without even the implied warranty of
 *    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 *    GNU General Public License for more details.
 *
 *    You should have received a copy of the GNU General Public Licensegit 
 *    along with TWLib.  If not, see <https://www.gnu.org/licenses/>.
 ******************************************************************************
 *
 *    Project available from here: https://github.com/martian73/TWLib.git
 ******************************************************************************
 */
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using TWLib.Models;
using TWLib.Streamer;
using TWLib.Streamer.Models;

namespace TWLib
{
    public class TWClient : IDisposable
    {
        public const string MinimumVersion = "1.0";
        public const string ClientVersion = "1.0";
        private string Username;
        private string Password;
        public static string APIurl = "https://api.tastyworks.com";
        private string _AuthToken;
        private Accounts _Accounts;
        private bool _Running = true;


        public Accounts Accounts
        {
            get
            {
                return _Accounts;
            }
        }

        private Thread DxfeedThread;
        private DxfeedStreamer DxfeedClient;
        public EventHandler<ServiceData[]> QuoteCallback = null;

        private STWStreamer TWStreamerClient;
        private MetaqueStreamer MetaqueueClient;
        private DateTime LoggedInAt;
        private DateTime LastValidate;
        private CookieContainer Cookies;
        private bool LoggedIn;
        private readonly string UserAgent = "okhttp/3.11.0";
        private Thread KeepAliveThread = null;

        private List<string> Watchlist;

        public void AddWatchlistSymbols(List<string> list)
        {
            Watchlist = Watchlist.Union(list).ToList();
            AddEquitySubscription(Watchlist, (int)ServiceDataType.QUOTE | (int)ServiceDataType.TRADE);
        }

        public void SetWatchlistSymbols(List<String> list)
        {
            Watchlist = list;
            AddEquitySubscription(list, (int)ServiceDataType.QUOTE | (int)ServiceDataType.TRADE);
        }

        public TWClient()
        {
            DxfeedThread = null;
            KeepAliveThread = null;
            LoggedIn = false;
            Cookies = new CookieContainer();
            _AuthToken = null;
            LoggedInAt = DateTime.MinValue;
            LastValidate = DateTime.MinValue;
            System.Net.ServicePointManager.Expect100Continue = false;
            Watchlist = new List<string>();
        }

        public string AuthToken
        {
            get
            {
                return String.Format(_AuthToken);
            }
        }

        #region Account Logon and Session maintenance     

        public bool Init(string username, string password)
        {
            Username = username;
            Password = password;
            _AuthToken = GetSessionToken();
            if (_AuthToken != null)
            {
                ValidateSession();
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Dispose()
        {
            Logout();
            if (KeepAliveThread != null && KeepAliveThread.ThreadState == System.Threading.ThreadState.Running)
                KeepAliveThread.Join();
            KeepAliveThread = null;
        }

        public void Logout()
        {
            LoggedIn = false;
            _AuthToken = null;
            Cookies = new CookieContainer();

            if (DxfeedClient != null && DxfeedClient.State == Streamer.Models.DxFeedStreamState.READY)
                DxfeedClient.Stop();

            if (KeepAliveThread != null && KeepAliveThread.ThreadState == System.Threading.ThreadState.Running)
                KeepAliveThread.Join();
            KeepAliveThread = null;
            Console.WriteLine("Logged out.");
        }

        private string GetSessionToken()
        {
            if (LoggedIn && _AuthToken != null)
            {
                return _AuthToken;
            }
            string result = null;
            HttpWebResponse response;
            Cookies = new CookieContainer();
            try
            {
                result = PostJsonRequest("/sessions", @"{""login"": """ + Username + @""", ""password"": """ + Password + @""" }", out response);
                if (response.Cookies != null)
                    Cookies.Add(response.Cookies);
            }
            catch
            {
                return null;
            }

            Session session = new Session();

            if (response.StatusCode != HttpStatusCode.Created)
            {
                LoggedIn = false;
                LoggedInAt = DateTime.MinValue;
                LastValidate = DateTime.MinValue;
                _AuthToken = null;
                JObject obj = new JObject();
                string message = (string)obj.SelectToken("error.message");
                throw new Exception(message);
            }

            if (result != null)
            {
                LoggedIn = true;
                JsonSerializer serializer = new JsonSerializer();
                session = JsonConvert.DeserializeObject<Session>(result);
                JObject ob = JObject.Parse(result);
                string token = session.Data.SessionToken;
                KeepAliveThread = new Thread(() =>
                {
                    KeepSessionAlive();
                });
                KeepAliveThread.Start();
                return token;
            }
            return null;
        }

        public void ValidateSession()
        {
            if (LoggedIn == false || _AuthToken == null)
            {
                LoggedInAt = DateTime.MinValue;
                LastValidate = DateTime.MinValue;
                return;
            }

            HttpWebResponse wresponse;
            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add("Authorization", _AuthToken);
            string response = PostJsonRequest("/sessions/validate", null, out wresponse, headers);

            if (wresponse.StatusCode != HttpStatusCode.Created)
            {
                LoggedIn = false;
                LoggedInAt = DateTime.MinValue;
                LastValidate = DateTime.MinValue;
                _AuthToken = null;
                JObject obj = new JObject();
                string message = (string)obj.SelectToken("error.message");
                throw new Exception(message);
            }
            LastValidate = DateTime.UtcNow;
            return;
        }

        // Keep revalidating every 20 minutes.
        // Loop every half second to keep exits snappy.     
        public void KeepSessionAlive()
        {
            while (LoggedIn)
            {
                if (LastValidate.AddMinutes(20) < DateTime.UtcNow)
                {
                    ValidateSession();
                }
                Thread.Sleep(500);
            }
        }

        void DxfeedQuoteCallback(object Sender, ServiceData[] data)
        {
            QuoteCallback?.Invoke(Sender, data);
        }

        public void DxFeedLoop()
        {
            Console.WriteLine("TWClient: Starting Dxfeed.");
            while (_Running)
            {
                DxfeedClient = new DxfeedStreamer();
                DxfeedClient.Init(_AuthToken);
                
                DxfeedClient.QuoteCallback += DxfeedQuoteCallback;

                Console.WriteLine("TWClient: Dxfeed ready.");

                if (Watchlist != null && Watchlist.Count > 0)
                    DxfeedClient.AddEquitySubscription(Watchlist, (int)ServiceDataType.QUOTE | (int)ServiceDataType.TRADE);

                do
                {
                    Thread.Sleep(100);
                } while (DxfeedClient.State == Streamer.Models.DxFeedStreamState.READY);

                Console.WriteLine("TWClient: Restarting Dxfeed.");
                CloseDxfeedStreamer();
            }
        }

        public void InitDxfeedStreamer()
        {
            if (!LoggedIn || _AuthToken == null)
                throw new Exception("TWClient: Not logged in.");

            if (DxfeedThread == null)
            {
                DxfeedThread = new Thread(() => { DxFeedLoop(); });
                DxfeedThread.Start();
            }
        }

        public void CloseDxfeedStreamer()
        {
            Console.WriteLine("TWClient: Closing DXFeed.");
            DxfeedClient.Stop();
            DxfeedClient = null;
        }

        public void InitStwStreamer()
        {
            if (!LoggedIn || _AuthToken == null)
                throw new Exception("TWClient: Not logged in.");

            TWStreamerClient = new STWStreamer();
            TWStreamerClient.Init(_AuthToken);
            _Accounts = GetAccounts();
            List<string> accountsId = new List<string>();

            foreach (Accounts.Item item in _Accounts.Data.Items)
            {
                accountsId.Add(item.Account.AccountNumber);
            }

            TWStreamerClient.AccountSubscribe(accountsId);
            TWStreamerClient.UserMessageSubscribe();
            TWStreamerClient.PublicWatchListSubscribe();
        }

        public void CloseStwStreamer()
        {
            Console.WriteLine("TWClient: Closing StwStreamer");
            TWStreamerClient.Stop();
            TWStreamerClient.Dispose();
            TWStreamerClient = null;
        }

        public void InitMetaqueueStreamer()
        {
            MetaqueueClient = new MetaqueStreamer();
            MetaqueueClient.Init(null);
        }

        public void CloseMetaqueStreamer()
        {
            Console.WriteLine("TWClient: Closing MetaqueueStreamer");
            MetaqueueClient.Stop();
            MetaqueueClient.Dispose();
            MetaqueueClient = null;
        }
        #endregion

        #region Account state
        public Accounts GetAccounts()
        {
            ValidateSession();
            if (LoggedIn == false)
                throw new Exception("TWClient: Not logged in");
            Accounts accounts = new Accounts();
            HttpWebResponse wresponse;
            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add("Authorization", _AuthToken);
            string response = GetJsonRequest("/customers/me/accounts", out wresponse, headers);

            if (wresponse.StatusCode != HttpStatusCode.OK)
            {
                LoggedIn = false;
                LoggedInAt = DateTime.MinValue;
                LastValidate = DateTime.MinValue;
                _AuthToken = null;
                JObject obj = new JObject();
                string message = (string)obj.SelectToken("error.message");
                throw new Exception(message);
            }
            JsonSerializer serializer = new JsonSerializer();
            //try
            //{     
            accounts = JsonConvert.DeserializeObject<Accounts>(response);
            //}
            //catch (Exception ex)
            //{
            //}      
            return accounts;
        }

        public Balances Balances(string accountName)
        {
            ValidateSession();

            if (LoggedIn == false)
                throw new Exception("Not logged in");

            Balances balances = new Balances();
            HttpWebResponse wresponse;
            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add("Authorization", _AuthToken);
            string response = GetJsonRequest("/accounts/" + accountName + "/balances", out wresponse, headers);

            if (wresponse.StatusCode != HttpStatusCode.OK)
            {
                LoggedIn = false;
                LoggedInAt = DateTime.MinValue;
                LastValidate = DateTime.MinValue;
                _AuthToken = null;
                JObject obj = new JObject();
                string message = (string)obj.SelectToken("error.message");
                throw new Exception(message);
            }
            JsonSerializer serializer = new JsonSerializer();
            //try
            //{          
            balances = JsonConvert.DeserializeObject<Balances>(response);
            //} 
            //catch (Exception ex)
            //{
            //}        
            return balances;
        }

        public OrdersLive OrdersLive(string accountName)
        {
            ValidateSession();

            if (LoggedIn == false)
                throw new Exception("Not logged in");

            OrdersLive OrdersLive = new OrdersLive();
            HttpWebResponse wresponse;
            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add("Authorization", _AuthToken);
            string response = GetJsonRequest("/accounts/" + accountName + "/orders/live", out wresponse, headers);

            if (wresponse.StatusCode != HttpStatusCode.OK)
            {
                LoggedIn = false;
                LoggedInAt = DateTime.MinValue;
                LastValidate = DateTime.MinValue;
                _AuthToken = null;
                JObject obj = new JObject();
                string message = (string)obj.SelectToken("error.message");
                throw new Exception(message);
            }
            JsonSerializer serializer = new JsonSerializer();
            //try
            //{
            OrdersLive = JsonConvert.DeserializeObject<OrdersLive>(response);
            //}
            //catch (Exception ex)
            //{
            //}         
            return OrdersLive;
        }
        public TradingStatus TradingStatus(string accountName)
        {
            ValidateSession();

            if (LoggedIn == false)
                throw new Exception("Not logged in");

            TradingStatus TradingStatus = new TradingStatus();
            HttpWebResponse wresponse;
            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add("Authorization", _AuthToken);
            string response = GetJsonRequest("/accounts/" + accountName + "/trading-status", out wresponse, headers);

            if (wresponse.StatusCode != HttpStatusCode.OK)
            {
                LoggedIn = false;
                LoggedInAt = DateTime.MinValue;
                LastValidate = DateTime.MinValue;
                _AuthToken = null;
                JObject obj = new JObject();
                string message = (string)obj.SelectToken("error.message");
                throw new Exception(message);
            }
            JsonSerializer serializer = new JsonSerializer();
            //try
            //{   
            TradingStatus = JsonConvert.DeserializeObject<TradingStatus>(response);
            //}
            //catch (Exception ex)
            //{
            //}      
            return TradingStatus;
        }

        public MarginReport MarginReport(string accountName)
        {
            ValidateSession();

            if (LoggedIn == false)
                throw new Exception("Not logged in");

            MarginReport MarginReport = new MarginReport();
            HttpWebResponse wresponse;
            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add("Authorization", _AuthToken);
            string response = GetJsonRequest("/margin/accounts/" + accountName + "/report", out wresponse, headers);

            if (wresponse.StatusCode != HttpStatusCode.OK)
            {
                LoggedIn = false;
                LoggedInAt = DateTime.MinValue;
                LastValidate = DateTime.MinValue;
                _AuthToken = null;
                JObject obj = new JObject();
                string message = (string)obj.SelectToken("error.message");
                throw new Exception(message);
            }
            JsonSerializer serializer = new JsonSerializer();
            //try
            //{         
            MarginReport = JsonConvert.DeserializeObject<MarginReport>(response);
            //}
            //catch (Exception ex)
            //{
            //}        
            return MarginReport;
        }
        public Positions Positions(string accountName, bool include_closed_positions = false)
        {
            ValidateSession();

            if (LoggedIn == false)
                throw new Exception("Not logged in");

            Positions positions = new Positions();
            HttpWebResponse wresponse;
            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add("Authorization", _AuthToken);
            string uri = String.Format("/accounts/{0}/positions?include_closed_positions={1}", accountName, include_closed_positions.ToString());
            string response = GetJsonRequest(uri, out wresponse, headers);

            if (wresponse.StatusCode != HttpStatusCode.OK)
            {
                LoggedIn = false;
                LoggedInAt = DateTime.MinValue;
                LastValidate = DateTime.MinValue;
                _AuthToken = null;
                JObject obj = new JObject();
                string message = (string)obj.SelectToken("error.message");
                throw new Exception(message);
            }
            JsonSerializer serializer = new JsonSerializer();
            //try
            //{        
            positions = JsonConvert.DeserializeObject<Positions>(response);
            //}
            //catch (Exception ex)
            //{
            //}   
            return positions;
        }
        public MarketMetrics MarketMetrics(string symbol)
        {
            ValidateSession();

            if (LoggedIn == false)
                throw new Exception("Not logged in");

            MarketMetrics metrics = new MarketMetrics();
            HttpWebResponse wresponse;
            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add("Authorization", _AuthToken);
            string uri = String.Format("/market-metrics?symbols={0}", symbol);
            string response = GetJsonRequest(uri, out wresponse, headers);

            if (wresponse.StatusCode != HttpStatusCode.OK)
            {
                LoggedIn = false;
                LoggedInAt = DateTime.MinValue;
                LastValidate = DateTime.MinValue;
                _AuthToken = null;
                JObject obj = new JObject();
                string message = (string)obj.SelectToken("error.message");
                throw new Exception(message);
            }
            //try
            //{       
            metrics = JsonConvert.DeserializeObject<MarketMetrics>(response);
            //}
            //catch (Exception ex)
            //{
            //}     
            return metrics;
        }

        public InstrumentsFutures InstrumentsFutures()
        {
            ValidateSession();

            if (LoggedIn == false)
                throw new Exception("Not logged in");

            InstrumentsFutures instruments = new InstrumentsFutures();
            HttpWebResponse wresponse;
            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add("Authorization", _AuthToken);
            string uri = "/instruments/futures";
            string response = GetJsonRequest(uri, out wresponse, headers);

            if (wresponse.StatusCode != HttpStatusCode.OK)
            {
                LoggedIn = false;
                LoggedInAt = DateTime.MinValue;
                LastValidate = DateTime.MinValue;
                _AuthToken = null;
                JObject obj = new JObject();
                string message = (string)obj.SelectToken("error.message");
                throw new Exception(message);
            }
            //try
            //{       
            instruments = JsonConvert.DeserializeObject<InstrumentsFutures>(response);
            //}
            //catch (Exception ex)
            //{
            //}     
            return instruments;
        }

        #endregion

        #region Searches and Queries  

        public SymbolSearch SearchSymbol(string symbol)
        {
            SymbolSearch search = new SymbolSearch();
            ValidateSession();

            if (LoggedIn == false)
                throw new Exception("Not logged in");

            HttpWebResponse wresponse;
            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add("Authorization", _AuthToken);
            string response = GetJsonRequest("/symbols/search/" + HttpUtility.UrlEncode(symbol), out wresponse, headers);

            if (wresponse.StatusCode != HttpStatusCode.OK)
            {
                LoggedIn = false;
                LoggedInAt = DateTime.MinValue;
                LastValidate = DateTime.MinValue;
                _AuthToken = null;
                JObject obj = new JObject();
                string message = (string)obj.SelectToken("error.message");
                throw new Exception(message);
            }
            JsonSerializer serializer = new JsonSerializer();
            //try
            //{          
            search = JsonConvert.DeserializeObject<SymbolSearch>(response);
            //}
            //catch (Exception ex)
            //{
            //}    
            return search;
        }
        public OptionChains GetChains(string symbol)
        {
            OptionChains chains = new OptionChains();
            ValidateSession();

            if (LoggedIn == false)
                throw new Exception("Not logged in");

            HttpWebResponse wresponse;
            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add("Authorization", _AuthToken);
            string response = GetJsonRequest("/option-chains/" + HttpUtility.UrlEncode(symbol) + "/nested", out wresponse, headers);

            if (wresponse.StatusCode != HttpStatusCode.OK)
            {
                LoggedIn = false;
                LoggedInAt = DateTime.MinValue;
                LastValidate = DateTime.MinValue;
                _AuthToken = null;
                JObject obj = new JObject();
                string message = (string)obj.SelectToken("error.message");
                throw new Exception(message);
            }
            JsonSerializer serializer = new JsonSerializer();
            //try
            //{           
            chains = JsonConvert.DeserializeObject<OptionChains>(response);
            //}
            //catch (Exception ex)
            //{
            //}        
            return chains;
        }
        #endregion
        public void AddEquitySubscription(List<string> symbols, int serviceDataFlags = 0)
        {
            if (symbols == null && symbols.Count == 0)
                return;

            Watchlist = Watchlist.Union(symbols).ToList();

            if (MetaqueueClient != null)
            {
                foreach (string symbol in symbols)
                    MetaqueueClient.SubscribeMarketMetrics(symbol);
            }
            if (DxfeedClient != null)
                DxfeedClient.AddEquitySubscription(Watchlist, serviceDataFlags);
        }

        public void AddOptionSubscription(List<string> symbols, int serviceDataFlags = 0)
        {
            if (symbols == null && symbols.Count == 0)
                return;
            Watchlist = Watchlist.Union(symbols).ToList();
            if (DxfeedClient != null)
                DxfeedClient.AddOptionSubscription(Watchlist, serviceDataFlags);
        }

        public Orders ExecuteOrder(string accountID, object order)
        {
            if (!(order is Order || order is OrderStop))
            {
                Trace.Write("Invalid order.");
                return null;
            }
            Orders orders = new Orders();
            ValidateSession();

            if (LoggedIn == false)
                throw new Exception("Not logged in");

            MarketMetrics metrics = new MarketMetrics();
            HttpWebResponse wresponse;
            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add("Authorization", _AuthToken);
            headers.Add("Accept-Encoding", "gzip, deflate");
            string uri = String.Format("/accounts/{0}/orders", accountID);
            string payload = JsonConvert.SerializeObject(order);
            string response = PostJsonRequest(uri, payload, out wresponse, headers);
            Trace.WriteLine(response);

            if (wresponse.StatusCode == HttpStatusCode.OK)
            {
                orders = JsonConvert.DeserializeObject<Orders>(response);
                return orders;
            }
            Orders err = new Orders();
            err = JsonConvert.DeserializeObject<Orders>(response);
            return err;
        }

        public CancelOrderRes CancelOrder(string accountID, string orderID)
        {
            CancelOrderRes res = new CancelOrderRes();
            ValidateSession();

            if (LoggedIn == false)
                throw new Exception("Not logged in");

            HttpWebResponse wresponse;
            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add("Authorization", _AuthToken);
            headers.Add("Accept-Encoding", "gzip, deflate");
            string uri = String.Format("/accounts/{0}/orders/{1}", accountID, orderID);
            string response = SendDeleteRequest(uri, out wresponse, headers);

            if (wresponse.StatusCode == HttpStatusCode.OK)
            {
                res = JsonConvert.DeserializeObject<CancelOrderRes>(response);
                return res;
            }
            return null;
        }

        public string SendDeleteRequest(string path, out HttpWebResponse response, WebHeaderCollection headers = null)
        {
            // create a request   
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(APIurl + path);
            request.KeepAlive = false;
            request.ProtocolVersion = HttpVersion.Version11;
            request.Method = "DELETE";
            // this is important - make sure you specify type this way        
            request.ContentType = "application/json; charset=UTF-8";
            request.Accept = "application/json, text/javascript";
            request.CookieContainer = Cookies;

            if (headers != null)
            {
                headers.Add("Accept-Version", "v1");
                request.Headers = headers;
            }
            request.UserAgent = UserAgent;
            // grab the response and print it out to the console along with the status code      
            string result;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                response = (HttpWebResponse)ex.Response;
                using (StreamReader rdr = new StreamReader(response.GetResponseStream()))
                {
                    result = rdr.ReadToEnd();
                    return result;
                }
            }
            if (response != null && response.Cookies != null)
            {
                Cookies.Add(response.Cookies);
            }
            using (StreamReader rdr = new StreamReader(response.GetResponseStream()))
            {
                result = rdr.ReadToEnd();
            }
            return result;
        }

        public string GetJsonRequest(string path, out HttpWebResponse response, WebHeaderCollection headers = null)
        {
            // create a request          
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(APIurl + path);
            request.KeepAlive = false;
            request.ProtocolVersion = HttpVersion.Version11;
            request.Method = "GET";
            // this is important - make sure you specify type this way      
            request.ContentType = "application/json; charset=UTF-8";
            request.Accept = "application/json, text/javascript";
            request.CookieContainer = Cookies;
            if (headers != null)
            {
                headers.Add("Accept-Version", "v1");
                request.Headers = headers;
            }
            request.UserAgent = UserAgent;
            // grab te response and print it out to the console along with the status code      
            string result;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                response = (HttpWebResponse)ex.Response;
                using (StreamReader rdr = new StreamReader(response.GetResponseStream()))
                {
                    result = rdr.ReadToEnd();
                    return result;
                }
            }
            if (response != null && response.Cookies != null)
            {
                Cookies.Add(response.Cookies);
            }
            using (StreamReader rdr = new StreamReader(response.GetResponseStream()))
            {
                result = rdr.ReadToEnd();
            }
            return result;
        }

        public string PostJsonRequest(string path, string json, out HttpWebResponse response, WebHeaderCollection headers = null)
        {
            // create a request       
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(APIurl + path);
            if (headers != null)
            {
                headers.Add("Accept-Version", "v1");
                request.Headers = headers;
            }
            request.UserAgent = UserAgent;
            request.KeepAlive = false;
            request.Method = "POST";
            request.ProtocolVersion = HttpVersion.Version11;
            // turn our request string into a byte stream      
            byte[] postBytes = null;
            if (json != null)
            {
                postBytes = Encoding.UTF8.GetBytes(json);
            }
            // this is important - make sure you specify type this way  
            request.ContentType = "application/json;charset=utf-8";
            request.Accept = "application/json, text/javascript";
            request.ContentLength = (postBytes != null) ? postBytes.Length : 0;
            request.CookieContainer = Cookies;
            Stream requestStream = request.GetRequestStream();
            if (postBytes != null)
            {
                // now send it            
                requestStream.Write(postBytes, 0, postBytes.Length);
            }
            requestStream.Close();
            string result;
            // grab the response and print it out to the console along with the status code    
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                if (response != null && response.Cookies != null)
                {
                    Cookies.Add(response.Cookies);
                }
            }
            catch (WebException ex)
            {
                response = (HttpWebResponse)ex.Response;
                using (StreamReader rdr = new StreamReader(response.GetResponseStream()))
                {
                    result = rdr.ReadToEnd();
                    return result;
                }
            }
            using (StreamReader rdr = new StreamReader(response.GetResponseStream()))
            {
                result = rdr.ReadToEnd();
            }
            return result;
        }
    }
}
