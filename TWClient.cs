using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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
    public class TWClient
    {
        public const string MinimumVersion = "1.0";
        public const string ClientVersion = "1.0";

        private string Username;
        private string Password;
        public static string APIurl = "https://api.tastyworks.com";
        private string AuthToken;

        private Accounts _Accounts;

        public Accounts Accounts {
            get
            {
                return _Accounts;
            }
        }

        public DxfeedStreamer DxfeedClient;
        public STWStreamer TWStreamerClient;
        public MetaqueStreamer MetaqueueClient;

        private DateTime LoggedInAt;
        private DateTime LastValidate;

        private CookieContainer Cookies;
        private bool LoggedIn;
        private string UserAgent = "okhttp/3.11.0";
        private Thread KeepAliveThread = null;
        
        public TWClient()
        {
            KeepAliveThread = null;
            LoggedIn = false;
            Cookies = new CookieContainer();
            AuthToken = null;
            LoggedInAt = DateTime.MinValue;
            LastValidate = DateTime.MinValue;
            System.Net.ServicePointManager.Expect100Continue = false;
        }


        #region Account Logon and Session maintenance

        public bool Init(string username, string password)
        {
            Username = username;
            Password = password;

            AuthToken = GetSessionToken();

            if (AuthToken != null)
            {
                ValidateSession();
            }
            else
            {

            }

            // Start keepalive thread
            return true;
        }

        public void Logout()
        {
            LoggedIn = false;
            AuthToken = null;
            Cookies = new CookieContainer();
            if (KeepAliveThread != null && KeepAliveThread.ThreadState == ThreadState.Running)
                KeepAliveThread.Join();
            KeepAliveThread = null;

            Console.WriteLine("Logged out.");
        }

        private string GetSessionToken()
        {
            if (LoggedIn && AuthToken != null)
            {
                return AuthToken;
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
                AuthToken = null;

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

                KeepAliveThread = new Thread(() => { KeepSessionAlive(); });
                KeepAliveThread.Start();
                
                return token;
            }

            return null;
        }

        public void ValidateSession()
        {
            if (LoggedIn == false || AuthToken == null)
            {
                LoggedInAt = DateTime.MinValue;
                LastValidate = DateTime.MinValue;
                return;
            }

            HttpWebResponse wresponse;
            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add("Authorization", AuthToken);

            string response = PostJsonRequest("/sessions/validate", null, out wresponse, headers);

            if (wresponse.StatusCode != HttpStatusCode.Created)
            {
                LoggedIn = false;
                LoggedInAt = DateTime.MinValue;
                LastValidate = DateTime.MinValue;
                AuthToken = null;

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


        public void InitDxfeedStreamer()
        {
            if (!LoggedIn || AuthToken == null)
                throw new Exception("Not logged in.");

            DxfeedClient = new DxfeedStreamer();
            DxfeedClient.Init(AuthToken);
        }

        public void CloseDxfeedStreamer()
        {
            Console.WriteLine("Closing DXFeed.");
            DxfeedClient.Stop();
            DxfeedClient.Dispose();
            DxfeedClient = null;
        }

        public void InitStwStreamer()
        {         
            if (!LoggedIn || AuthToken == null)
                throw new Exception("Not logged in.");

            TWStreamerClient = new STWStreamer();
            TWStreamerClient.Init(AuthToken);

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
            Console.WriteLine("Closing StwStreamer");
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
            Console.WriteLine("Closing MetaqueueStreamer");
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
                throw new Exception("Not logged in");

            Accounts accounts = new Accounts();

            HttpWebResponse wresponse;
            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add("Authorization", AuthToken);

            string response = GetJsonRequest("/customers/me/accounts", out wresponse, headers);

            if (wresponse.StatusCode != HttpStatusCode.OK)
            {
                LoggedIn = false;
                LoggedInAt = DateTime.MinValue;
                LastValidate = DateTime.MinValue;
                AuthToken = null;

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
            //  
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
            headers.Add("Authorization", AuthToken);

            string response = GetJsonRequest("/accounts/" + accountName + "/balances", out wresponse, headers);

            if (wresponse.StatusCode != HttpStatusCode.OK)
            {
                LoggedIn = false;
                LoggedInAt = DateTime.MinValue;
                LastValidate = DateTime.MinValue;
                AuthToken = null;

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
            //  
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
            headers.Add("Authorization", AuthToken);

            string response = GetJsonRequest("/accounts/" + accountName + "/orders/live", out wresponse, headers);

            if (wresponse.StatusCode != HttpStatusCode.OK)
            {
                LoggedIn = false;
                LoggedInAt = DateTime.MinValue;
                LastValidate = DateTime.MinValue;
                AuthToken = null;

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
            //  
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
            headers.Add("Authorization", AuthToken);

            string response = GetJsonRequest("/accounts/" + accountName + "/trading-status", out wresponse, headers);

            if (wresponse.StatusCode != HttpStatusCode.OK)
            {
                LoggedIn = false;
                LoggedInAt = DateTime.MinValue;
                LastValidate = DateTime.MinValue;
                AuthToken = null;

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
            //  
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
            headers.Add("Authorization", AuthToken);

            string response = GetJsonRequest("/margin/accounts/" + accountName + "/report", out wresponse, headers);

            if (wresponse.StatusCode != HttpStatusCode.OK)
            {
                LoggedIn = false;
                LoggedInAt = DateTime.MinValue;
                LastValidate = DateTime.MinValue;
                AuthToken = null;

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
            //  
            //}
            return MarginReport;
        }

        public Positions Positions(string accountName, bool include_closed_positions=false)
        {
            ValidateSession();

            if (LoggedIn == false)
                throw new Exception("Not logged in");

            Positions positions = new Positions();

            HttpWebResponse wresponse;
            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add("Authorization", AuthToken);

            string uri = String.Format("/accounts/{0}/positions?include_closed_positions={1}", accountName, include_closed_positions.ToString()); 
            string response = GetJsonRequest(uri, out wresponse, headers);

            if (wresponse.StatusCode != HttpStatusCode.OK)
            {
                LoggedIn = false;
                LoggedInAt = DateTime.MinValue;
                LastValidate = DateTime.MinValue;
                AuthToken = null;

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
            //  
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
            headers.Add("Authorization", AuthToken);

            string uri = String.Format("/market-metrics?symbols={0}", symbol);
            string response = GetJsonRequest(uri, out wresponse, headers);

            if (wresponse.StatusCode != HttpStatusCode.OK)
            {
                LoggedIn = false;
                LoggedInAt = DateTime.MinValue;
                LastValidate = DateTime.MinValue;
                AuthToken = null;

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
            //  
            //}
            return metrics;
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
            headers.Add("Authorization", AuthToken);

            string response = GetJsonRequest("/symbols/search/" + HttpUtility.UrlEncode(symbol), out wresponse, headers);

            if (wresponse.StatusCode != HttpStatusCode.OK)
            {
                LoggedIn = false;
                LoggedInAt = DateTime.MinValue;
                LastValidate = DateTime.MinValue;
                AuthToken = null;

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
            //  
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
            headers.Add("Authorization", AuthToken);

            string response = GetJsonRequest("/option-chains/" + HttpUtility.UrlEncode(symbol) + "/nested", out wresponse, headers);

            if (wresponse.StatusCode != HttpStatusCode.OK)
            {
                LoggedIn = false;
                LoggedInAt = DateTime.MinValue;
                LastValidate = DateTime.MinValue;
                AuthToken = null;

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
            //  
            //}
            return chains;
        }
        #endregion

        public void AddSymbolsSubscription(List<string> symbols)
        {
            DxfeedClient.AddSymbolsSubscription(symbols);

            foreach (string symbol in symbols)
                MetaqueueClient.SubscribeMarketMetrics(symbol);
        }

        public Orders ExecuteOrder(string accountID, Order order)
        {
            Orders orders = new Orders();

            ValidateSession();

            if (LoggedIn == false)
                throw new Exception("Not logged in");

            MarketMetrics metrics = new MarketMetrics();

            HttpWebResponse wresponse;
            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add("Authorization", AuthToken);
            headers.Add("Accept-Encoding", "gzip, deflate");
            string uri = String.Format("/accounts/{0}/orders", accountID);
            string payload = JsonConvert.SerializeObject(order);
            string response = PostJsonRequest(uri, payload, out wresponse, headers);
           
            if (wresponse.StatusCode == HttpStatusCode.OK)
            {
                orders = JsonConvert.DeserializeObject<Orders>(response);
                return orders;
            }

            Orders err = new Orders();
            err = JsonConvert.DeserializeObject<Orders>(response);
            return err;
        }

        public string GetJsonRequest(string path, out HttpWebResponse response, WebHeaderCollection headers = null)
        {
            // create a request
            HttpWebRequest request = (HttpWebRequest)
            WebRequest.Create(APIurl + path); request.KeepAlive = false;
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
            request.ContentType = "application/json; charset=utf-8";
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

            // grab te response and print it out to the console along with the status code
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
