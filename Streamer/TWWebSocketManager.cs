using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Net;
using System.IO;
using TWLib.Streamer.Models;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace TWLib.Streamer
{
    public abstract class TWWebSocketManager
    {
        public TWWebSocketManager()
        {
            StreamActive = false;
            Cookies = new CookieContainer();
            Token = TokenSource.Token;
            StreamerSocket = new ClientWebSocket();
        }

        ClientWebSocket StreamerSocket;

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
        private readonly SemaphoreSlim SendLock = new SemaphoreSlim(1);
        private CancellationToken Token;
        private CancellationTokenSource TokenSource = new CancellationTokenSource();

        protected Thread HeartBeatThread { get; set; }

        protected abstract void HeartBeatLoop();

        protected void Start()
        {
            StreamerSocket = new ClientWebSocket();
            StreamerSocket.ConnectAsync(new System.Uri(StreamerWebsocketUrl),
                CancellationToken.None).ContinueWith(AfterConnect);
        }

        public Func<Task> ServerConnected;
        public Func<Task> ServerDisconnected;

    
        private void AfterConnect(Task connectTask)
        {
            if (connectTask.IsCompleted)
            {
                Task.Run(async () =>
                {
                    StreamActive = true;
                    ServerConnected?.Invoke();
                    await DataReceiver(Token);
                }, Token);
            }
            else
            {
                StreamActive = false;
                ServerDisconnected?.Invoke();
            }
        }

        private async Task DataReceiver(CancellationToken? cancelToken = null)
        {
            cancelToken = cancelToken ?? CancellationToken.None;
            try
            {
                #region Wait-for-Data

                while (true)
                {
                    cancelToken?.ThrowIfCancellationRequested();

                    byte[] data = await MessageReadAsync(cancelToken.Value);
                    Task unawaited = Task.Run(() => MessageReceived(data), Token);
                }

                #endregion
            }
            catch (OperationCanceledException)
            {
            }
            catch (WebSocketException)
            {
            }
            catch (Exception e)
            {
                Console.WriteLine("*** DataReceiver server " + StreamerWebsocketUrl + " disconnected");
                Console.WriteLine("Exception: DataReceiver" + e.Message + "\r\n" + e.StackTrace);
            }
            finally
            {
                StreamActive = false;
                ServerDisconnected?.Invoke();
            }
        }

        private async Task<byte[]> MessageReadAsync(CancellationToken token)
        {
            /*
             *
             * Do not catch exceptions, let them get caught by the data reader
             * to destroy the connection
             *
             */

            if (StreamerSocket == null)
                return null;

            byte[] buffer = new byte[65536];
            byte[] data = null;

            WebSocketReceiveResult receiveResult = null;

            using (MemoryStream dataMs = new MemoryStream())
            {
                buffer = new byte[65536];
                ArraySegment<byte> bufferSegment = new ArraySegment<byte>(buffer);

                while (StreamerSocket.State == WebSocketState.Open)
                {
                    receiveResult = await StreamerSocket.ReceiveAsync(bufferSegment, token);
                    if (receiveResult.Count > 0)
                    {
                        await dataMs.WriteAsync(buffer, 0, receiveResult.Count);
                    }

                    if (receiveResult.EndOfMessage
                        || receiveResult.CloseStatus == WebSocketCloseStatus.Empty
                        || receiveResult.MessageType == WebSocketMessageType.Close)
                    {
                        data = dataMs.ToArray();
                        break;
                    }
                }
            }
            return data;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                TokenSource.Cancel();
                StreamerSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "closing", new CancellationToken(false));
            }
        }

        public virtual void SendRequest(TWRequest request)
        {
            if (!StreamActive)
                throw new Exception("Stream not active.");

            string json = request.Serialize();
            byte [] postBytes = Encoding.UTF8.GetBytes(json);
            StreamerSocket.SendAsync(new System.ArraySegment<byte>(postBytes), WebSocketMessageType.Text, true, CancellationToken.None);

            Console.WriteLine("Sending: " + json);
        }

        public abstract void ReceiveResponse(string response);

        private async Task MessageReceived(byte[] buffer)
        {
            if (buffer != null)
            {
                string retval = Encoding.UTF8.GetString(buffer);
                await Task.Run(() => { ReceiveResponse(retval); });
            }
        }

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
