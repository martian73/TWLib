/*   This file is part of TWLib.
 *
 *    TWLib is free software: you can redistribute it and/or modify
 *    it under the terms of the GNU General Public License as published by
 *    the Free Software Foundation, either version 3 of the License, or
 *    (at your option) any later version.
 *
 *    TWLib is distributed in the hope that it will be useful,
 *    but WITHOUT ANY WARRANTY; without even the implied warranty of
 *    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 *    GNU General Public License for more details.
 *
 *    You should have received a copy of the GNU General Public License
 *    along with TWLib.  If not, see <https://www.gnu.org/licenses/>.
 ******************************************************************************
 *
 *    Project available from here: https://github.com/martian73/TWLib.git
 ******************************************************************************
 */
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TWLib.Streamer.Models;
using WebSocketSharp;

namespace TWLib.Streamer
{
    public abstract class TWWebSocketManager : IDisposable
    {
        public TWWebSocketManager()
        {
            StreamActive = false;
            Cookies = new CookieContainer();
            Token = TokenSource.Token;
        }

        WebSocketSharp.WebSocket StreamerSocket;

        private CookieContainer Cookies;
        private readonly string UserAgent = "okhttp/3.11.0";
        private Thread RunLoopThread = null;

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
        private CancellationToken Token;
        private CancellationTokenSource TokenSource = new CancellationTokenSource();

        protected Thread HeartBeatThread { get; set; }
        protected Thread MessagePumpThread { get; set; }
        private Notifier nf;

        protected abstract void HeartBeatLoop();
        public abstract void Init(string authToken);
        public abstract void Restart();

        public virtual void Stop()
        {
            StreamActive = false;
            RunLoopThread = null;
        }

        protected void Start()
        {
            if (RunLoopThread != null)
                return;

            RunLoopThread = new Thread(() => { RunLoop(); });
            RunLoopThread.Start();
        }

        public void SendPing()
        {
            if (StreamerSocket != null)
            {
                StreamerSocket.Ping();
            }
        }

        private void RunLoop()
        {

            Console.WriteLine("TWWebSocketManager: EnteringRunLoop");

            using (nf = new Notifier())
            using (StreamerSocket = new WebSocketSharp.WebSocket(StreamerWebsocketUrl))
            {
                StreamActive = true;
                StreamerSocket.WaitTime = new TimeSpan(0, 0, 0, 0, 10000);
                StreamerSocket.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;
                StreamerSocket.OnMessage += (sender, e) => nf.Notify(e.Data);
                StreamerSocket.OnError += StreamerSocket_OnError;
                StreamerSocket.OnClose += StreamerSocket_OnClose;
                StreamerSocket.OnOpen += StreamerSocket_OnOpen;
                StreamerSocket.Connect();
                nf.OnMessage = OnMessage;
                while (StreamActive)
                {
                    Thread.Sleep(100);
                }

                Console.WriteLine("TWWebSocketManager: Leaving RunLoop");
                RunLoopThread = null;
                StreamerSocket.Close();
            }
        }

        private void StreamerSocket_OnOpen(object sender, EventArgs e)
        {
            Console.WriteLine("Stream Open");
            ServerConnected?.Invoke(sender, e);
            StreamActive = true;
        }

        private void StreamerSocket_OnClose(object sender, CloseEventArgs e)
        {
            Console.WriteLine("Stream closed.");
            ServerDisconnected?.Invoke(sender, e);
            StreamActive = false;
            //RunLoopThread.Join();
        }

        private void StreamerSocket_OnError(object sender, WebSocketSharp.ErrorEventArgs e)
        {
            Console.WriteLine("Stream error: " + e.Message);
            StreamActive = false;
        }

        private void OnMessage(string str)
        {
            this.ReceiveResponse(str);
        }

        public event EventHandler ServerConnected;
        public event EventHandler ServerDisconnected;


        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                TokenSource.Cancel();
                StreamerSocket.Close();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void SendRequest(TWRequest request)
        {
            if (!StreamActive)
                throw new Exception("Stream not active.");

            if (request is DxfeedRequest)
            {
                if (((DxfeedRequest)request).Id == null)
                {
                    Console.WriteLine("Null id!!!!!");
                }
            }

            string json = request.Serialize();
            StreamerSocket.Send(json);

            if (request.StreamType != StreamType.DXFEED)
                Console.WriteLine("Sending " + request.StreamType.ToString() + ": " + json);
        }

        public virtual void SendRawRequest(string request)
        {
            if (!StreamActive)
                throw new Exception("Stream not active.");

            byte[] postBytes = Encoding.UTF8.GetBytes(request);
            StreamerSocket.Send(request);

            Console.WriteLine("Sending Raw: " + request);
        }

        public virtual void SendRawRequest(byte[] postBytes)
        {
            if (!StreamActive)
                throw new Exception("Stream not active.");

            StreamerSocket.Send(postBytes);

            Console.WriteLine("Sending Raw: " + Encoding.UTF8.GetString(postBytes));
        }

        public abstract void ReceiveResponse(string response);



        public string GetJsonRequest(string apiUrl, string path, out HttpWebResponse response, WebHeaderCollection headers = null)
        {
            // create a request
            HttpWebRequest request = (HttpWebRequest)
            WebRequest.Create(apiUrl + path);
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
    }
}

