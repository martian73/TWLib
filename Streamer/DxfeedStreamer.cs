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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using TWLib.Streamer.Models;

namespace TWLib.Streamer
{
    public class DxConvo
    {
        public DateTime Sent;
        public DateTime Received;
        public DxfeedRequest Request;
        public DxfeedResponse Response;

        public DxConvo()
        {
            Sent = DateTime.MinValue;
            Received = DateTime.MinValue;
            Request = null;
            Response = null;
        }

        public bool Completed
        {
            get
            {
                return (Request != null && Response != null);
            }
        }
    }

    public class DxfeedStreamer : TWWebSocketManager, IDisposable
    {
        /// <summary>
        /// Conversation, request and response, grouped by id number
        /// </summary>
        public Dictionary<int, DxConvo> DxfeedConversations;

        /// <summary>
        /// Client Identification string
        /// </summary>
        private string ClientID;

        /// <summary>
        /// Authorization Token
        /// </summary>
        private string AuthToken;

        private DateTime LastValidate;

        private StreamerTokens StreamTokens;

        private DxFeedStreamState _State = DxFeedStreamState.NONE;

        public EventHandler<ServiceData[]> QuoteCallback = null;

        protected void DefaultQuoteCallback(ServiceData[] quote)
        {
        }

        public DxFeedStreamState State
        {
            get
            {
                return _State;
            }
        }

        /// <summary>
        /// Interval time
        /// </summary>
        private int Interval = 0;

        /// <summary>
        /// Timeout in milliseconds
        /// </summary>
        private int Timeout = 300000;

        public DxfeedStreamer()
        {
            StreamActive = false;
            DxfeedConversations = new Dictionary<int, DxConvo>();
        }

        protected override void HeartBeatLoop()
        {
            TimeSpan heartBeatSpan = new TimeSpan(0, 0, 10);
            DateTime lastBeat = DateTime.UtcNow;

            Console.WriteLine("DxFeed HeartBeatLoop starting.");
            while (StreamActive)
            {
                
                if (lastBeat.Add(heartBeatSpan) < DateTime.UtcNow)
                {
                    DxfeedMetaConnectReq req = new DxfeedMetaConnectReq(ClientID, 1);
                    SendRequest(req);
                    lastBeat = DateTime.UtcNow;
                }
                Thread.Sleep(10);

            }
            Console.WriteLine("Exiting DxFeedStreamer HeartBeatLoop.");
        }

        public override void Init(string authToken)
        {
            if (StreamActive)
                return;

            AuthToken = authToken;

            _State = DxFeedStreamState.HANDSHAKE;

            StreamTokens = GetQuoteStreamerTokens();
            StreamerWebsocketUrl = (StreamTokens.Data.WebsocketUrl + "/cometd").Replace("https://", "wss://");
            StreamerApiUrl = StreamTokens.Data.StreamerUrl;
            ServerConnected += DxfeedStreamer_ServerConnected;
            ServerDisconnected += DxfeedStreamer_ServerDisconnected;

            Start();

            while(_State != DxFeedStreamState.READY)
            {
                Thread.Sleep(100);
            }
        }

        private void DxfeedStreamer_ServerDisconnected(object sender, EventArgs e)
        {
            Console.WriteLine("DxFeed disconnected.");
        }

        private void DxfeedStreamer_ServerConnected(object sender, EventArgs e)
        {
            Console.WriteLine("DxFeed connected.");
            DxfeedMetaHandshakeReq req = new DxfeedMetaHandshakeReq(0, 60000, StreamTokens.Data.Token);
            SendRequest(req);
        }

        public override void Restart()
        {
            StreamActive = false;
            _State = DxFeedStreamState.NONE;

            // Wait for heartbeat thread to exit
            HeartBeatThread.Join();
            HeartBeatThread = null;

            Init(AuthToken);
        }

        public override void Stop()
        {
            StreamActive = false;
            _State = DxFeedStreamState.NONE;

            HeartBeatThread.Join();
            HeartBeatThread = null;

            Console.WriteLine("Exiting Dxfeed.");
        }

        public void AddEquitySubscription(List<string> symbols)
        {

            DxfeedServiceSubAddEquityReq req = new DxfeedServiceSubAddEquityReq(ClientID, symbols);
            SendRequest(req);
        }

        public void AddFutureSubscription(List<string> symbols)
        {
            DxfeedServiceSubAddEquityReq req = new DxfeedServiceSubAddEquityReq(ClientID, symbols);
            SendRequest(req);
        }

        public void AddOptionSubscription(List<string> options)
        {
            DxfeedServiceSubAddOptionReq req = new DxfeedServiceSubAddOptionReq(ClientID, options);
            SendRequest(req);
        }


        public override void SendRequest(TWRequest request)
        {
            if (request is DxfeedRequest)
            {
                if (request.StreamType != StreamType.DXFEED)
                    throw new Exception("Invalid request type");

                // Add it to the list of conversations
                int id = GetNextRequestID();
                ((DxfeedRequest)request).Id = id.ToString();
                //Console.WriteLine("DxFeed request: " + JsonConvert.SerializeObject(request));

                // Call base to send it through the web socket
                base.SendRequest(request);
            }
            else
            {
                throw new Exception("Not a DXfeed Request.");
            }
        }

        public override void ReceiveResponse(string response)
        {
            if (!StreamActive)
                return;

            // start with serializing the base object, DxfeedResponse, and switch on channel
            List<object> resArr = JsonConvert.DeserializeObject<List<object>>(response);

            foreach (object obj in resArr)
            {
                string root = obj.ToString();
                DxfeedResponse res = (DxfeedResponse)JsonConvert.DeserializeObject<DxfeedResponse>(root);

                if (res.Error != null && res.Error.Length > 0)
                {
                    Console.WriteLine("Error: " + res.Error);
                    continue;
                }

                try
                {
                    Console.WriteLine(DateTime.UtcNow.ToString("u") + " Received: \r\n" + response);

                    switch (res.Channel)
                    {
                        case DxfeedChannel.METAHANDSHAKE:
                            Console.WriteLine("/meta/handshake");
                            DxfeedMetaHandshakeRes hsRes = JsonConvert.DeserializeObject<DxfeedMetaHandshakeRes>(root);
                            ClientID = hsRes.ClientId;
                            DxfeedMetaConnectReq req = new DxfeedMetaConnectReq(ClientID, 0);
                            SendRequest(req);
                            break;
                        case DxfeedChannel.METACONNECT:
                            Console.WriteLine("/meta/connect");
                            DxfeedMetaConnectRes mcr = JsonConvert.DeserializeObject<DxfeedMetaConnectRes>(root);
                            if (!mcr.Successful)
                                throw new Exception("Failed connect.");
                            Interval = mcr.Advice.Interval;
                            Timeout = mcr.Advice.Timeout;
                            _State = DxFeedStreamState.READY;

                            if (HeartBeatThread == null)
                            {
                                HeartBeatThread = new Thread(() => { HeartBeatLoop(); });
                                HeartBeatThread.Start();
                            }
                            break;
                        case DxfeedChannel.SERVICEDATA:
                            DxfeedServiceDataRes dxSerDataRes = JsonConvert.DeserializeObject<DxfeedServiceDataRes>(root);
                            QuoteCallback?.Invoke(this, dxSerDataRes.ServiceData);
                            Console.WriteLine("/service/data");
                            break;
                        case DxfeedChannel.SERVICESTATE:
                            Console.WriteLine("/service/state");
                            break;
                        case DxfeedChannel.SERVICESUB:
                            Console.WriteLine("/service/sub");
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error Rx: " + ex.Message);
                    Console.WriteLine("Request: " + response);
                    Restart();
                }
            }

        }

        private StreamerTokens GetQuoteStreamerTokens()
        {
            StreamerTokens tokens = new StreamerTokens();

            HttpWebResponse wresponse;
            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add("Authorization", AuthToken);

            string response = GetJsonRequest(TWLib.TWClient.APIurl, "/quote-streamer-tokens", out wresponse, headers);

            if (wresponse.StatusCode != HttpStatusCode.OK)
            {
                StreamActive = false;
                LastValidate = DateTime.MinValue;
                AuthToken = null;

                JObject obj = new JObject();
                string message = (string)obj.SelectToken("error.message");
                throw new Exception(message);
            }

            JsonSerializer serializer = new JsonSerializer();
            //try
            //{
            tokens = JsonConvert.DeserializeObject<StreamerTokens>(response);
            //}
            //catch (Exception ex)
            //{
            //  
            //}
            return tokens;
        }
    }
}

