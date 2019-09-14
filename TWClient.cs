using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TWLib.Models;

namespace TWLib
{
    public class TWClient
    {
        string Username;
        string Password;
        string APIurl;
        string TokenString;
        DateTime LoggedInAt;
        DateTime LastValidate;

        CookieContainer Cookies;
        bool LoggedIn;
        string UserAgent = "okhttp/3.11.0";

        public TWClient()
        {
            LoggedIn = false;
            Cookies = new CookieContainer();
            TokenString = null;
            LoggedInAt = DateTime.MinValue;
            LastValidate = DateTime.MinValue;
        }

        public bool Init(string username, string password, string apiurl = "https://api.tastyworks.com")
        {
            Username = username;
            Password = password;
            APIurl = apiurl;

            TokenString = GetSessionToken();

            if (TokenString != null)
            {
                ValidateSession();
            }
            else
            {

            }
            return true;
        }

        private string GetSessionToken()
        {
            if (LoggedIn && TokenString != null)
            {
                return TokenString;
            }

            string result = null;
            HttpWebResponse response;

            try
            {
                result = PostJsonRequest("/sessions", @"{""login"": """ + Username + @""", ""password"": """ + Password + @""" }", out response);
            }
            catch
            {
                return null;
            }

            if (result != null)
            {
                JObject ob = JObject.Parse(result);

                string token = (string)ob.SelectToken("data.session-token");
                LoggedIn = true;
                LoggedInAt = DateTime.UtcNow;

                return token;
            }

            return null;
        }

        public void ValidateSession()
        {
            if (LoggedIn == false || TokenString == null)
            {
                LoggedInAt = DateTime.MinValue;
                LastValidate = DateTime.MinValue;
                return;
            }

            HttpWebResponse wresponse;
            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add("Authorization", TokenString);

            string response = PostJsonRequest("/sessions/validate", null, out wresponse, headers);

            if (wresponse.StatusCode != HttpStatusCode.Created)
            {
                LoggedIn = false;
                LoggedInAt = DateTime.MinValue;
                LastValidate = DateTime.MinValue;
                TokenString = null;

                JObject obj = new JObject();
                string message = (string)obj.SelectToken("error.message");
                throw new Exception(message);
            }

            LastValidate = DateTime.UtcNow;
            return;
        }


        public Accounts GetAccounts()
        {
            ValidateSession();

            if (LoggedIn == false)
                throw new Exception("Not logged in");

            Accounts accounts = new Accounts();

            HttpWebResponse wresponse;
            WebHeaderCollection headers = new WebHeaderCollection();

            headers.Add("Authorization", TokenString);

            string response = GetJsonRequest("/customers/me/accounts", out wresponse, headers);

            if (wresponse.StatusCode != HttpStatusCode.OK)
            {
                LoggedIn = false;
                LoggedInAt = DateTime.MinValue;
                LastValidate = DateTime.MinValue;
                TokenString = null;

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
            headers.Add("Authorization", TokenString);

            string response = GetJsonRequest("/accounts/" + accountName + "/balances", out wresponse, headers);

            if (wresponse.StatusCode != HttpStatusCode.OK)
            {
                LoggedIn = false;
                LoggedInAt = DateTime.MinValue;
                LastValidate = DateTime.MinValue;
                TokenString = null;

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

        public string GetJsonRequest(string path, out HttpWebResponse response, WebHeaderCollection headers = null)
        {
            // create a request
            HttpWebRequest request = (HttpWebRequest)
            WebRequest.Create(APIurl + path); request.KeepAlive = false;
            request.ProtocolVersion = HttpVersion.Version10;
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
            response = (HttpWebResponse)request.GetResponse();

            string result;
            using (StreamReader rdr = new StreamReader(response.GetResponseStream()))
            {
                result = rdr.ReadToEnd();
            }

            return result;
        }

        public string PostJsonRequest(string path, string json, out HttpWebResponse response, WebHeaderCollection headers = null)
        { 
            // create a request
            HttpWebRequest request = (HttpWebRequest)
            WebRequest.Create(APIurl + path); request.KeepAlive = false;
            request.ProtocolVersion = HttpVersion.Version10;
            request.Method = "POST";


            // turn our request string into a byte stream
            byte[] postBytes = null;
            if (json != null)
            {
                postBytes = Encoding.UTF8.GetBytes(json);
            }

            // this is important - make sure you specify type this way
            request.ContentType = "application/json; charset=UTF-8";
            request.Accept = "application/json, text/javascript";
            request.ContentLength = (postBytes != null ) ? postBytes.Length : 0;
            request.CookieContainer = Cookies;
            if (headers != null)
            {
                headers.Add("Accept-Version", "v1");
                request.Headers = headers;
            }
            request.UserAgent = UserAgent;

            Stream requestStream = request.GetRequestStream();

            if (postBytes != null)
            {
                // now send it
                requestStream.Write(postBytes, 0, postBytes.Length);
            }
            requestStream.Close();

            // grab te response and print it out to the console along with the status code
            response = (HttpWebResponse)request.GetResponse();
            
            string result;
            using (StreamReader rdr = new StreamReader(response.GetResponseStream()))
            {
                result = rdr.ReadToEnd();
            }

            return result;
        }
    }
}
