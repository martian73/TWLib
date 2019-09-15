using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Net;
using System.IO;

namespace TWLib.Streamer
{
    public abstract class TWWebSocketManager
    {
        public TWWebSocketManager()
        {
            StreamActive = false;
            Cookies = new CookieContainer();
        }
        
        private CookieContainer Cookies;
        private string UserAgent = "okhttp/3.11.0";

        protected string StreamerApiUrl { get; set; }

        protected string StreamerWebsocketUrl { get; set; }

        private int CurrentRequestID = 1;

        protected int GetNextRequestID()
        {
            int id = CurrentRequestID;
            CurrentRequestID++;
            return id;
        }

        protected bool StreamActive { get; set; }
  
        protected Thread HeartBeatThread { get; set; }

        protected abstract void HeartBeatLoop();

        //protected abstract Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken);

        //protected abstract Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken);

        public string GetJsonRequest(string apiUrl, string path, out HttpWebResponse response, WebHeaderCollection headers = null)
        {
            // create a request
            HttpWebRequest request = (HttpWebRequest)
            WebRequest.Create(apiUrl + path); request.KeepAlive = false;
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
    }
}
