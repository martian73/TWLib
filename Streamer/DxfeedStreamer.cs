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
            while (StreamActive)
            {
                Thread.Sleep(500);
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


            HeartBeatThread = new Thread(() => { HeartBeatLoop(); });
            HeartBeatThread.Start();

            Start();

            while (!StreamActive)
                Thread.Sleep(100);

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

        public void AddSymbolsSubscription(List<string> symbols)
        {
            DxfeedServiceSubAddReq req = new DxfeedServiceSubAddReq(ClientID, symbols);
            SendRequest(req);
        }

        public override void SendRequest(TWRequest request)
        {
            if (request.StreamType != StreamType.DXFEED)
                throw new Exception("Invalid request type");

            // Add it to the list of conversations
            DxfeedRequest dxReq = (DxfeedRequest)request;
            int id = GetNextRequestID();
            dxReq.Id = id.ToString();
            DxConvo convo = new DxConvo();
            convo.Sent = DateTime.UtcNow;
            convo.Request = (DxfeedRequest)request;
            DxfeedConversations.Add(id, convo);

            // Call base to send it through the web socket
            base.SendRequest(request);
        }

        public void HandleConversation(int id)
        {
            DxConvo convo = DxfeedConversations[id];
            switch (convo.Response.Channel)
            {
                case DxfeedChannel.METAHANDSHAKE:
                    Console.WriteLine("Convo Channel: /meta/handshake: Setting clientID");
                    ClientID = ((DxfeedMetaHandshakeRes)convo.Response).ClientId;
                    DxfeedMetaConnectReq req = new DxfeedMetaConnectReq(ClientID, 0);
                    SendRequest(req);
                    _State = DxFeedStreamState.CONNECT;
                    break;
                case DxfeedChannel.METACONNECT:
                    Console.WriteLine("Convo Channel: /meta/connect: Setting Interval and Timeout");
                    DxfeedMetaConnectRes mcr = (DxfeedMetaConnectRes)convo.Response;
                    if (!mcr.Successful)
                        throw new Exception("Failed connect.");
                    Interval = mcr.Advice.Interval;
                    Timeout = mcr.Advice.Timeout;
                    _State = DxFeedStreamState.READY;
                    break;
                case DxfeedChannel.SERVICEDATA:
                    Console.WriteLine("Convo Channel: /service/data");
                    break;
                case DxfeedChannel.SERVICESTATE:
                    Console.WriteLine("Convo Channel: /service/state");
                    break;
                case DxfeedChannel.SERVICESUB:
                    Console.WriteLine("Convo Channel: /service/sub");
                    break;
                default:
                    throw new Exception("Unhandled channel");
            }
            DxfeedConversations.Remove(id);
        }

        public override void ReceiveResponse(string response)
        {
            // start with serializing the base object, DxfeedResponse, and switch on channel
            DxfeedResponse[] resArr = JsonConvert.DeserializeObject<DxfeedResponse[]>(response);

            if (resArr.Length != 1)
                throw new Exception("Not expecting multiple elements in the array");

            DxfeedResponse res = resArr[0];
            DxfeedResponse finalRes = res;

            switch (res.Channel)
            {
                case DxfeedChannel.METAHANDSHAKE:
                    Console.WriteLine("/meta/handshake");
                    resArr = JsonConvert.DeserializeObject<DxfeedMetaHandshakeRes[]>(response);
                    if (resArr.Length != 1)
                        throw new Exception("Not expecting multiple elements in the array");
                    finalRes = resArr[0];
                    break;
                case DxfeedChannel.METACONNECT:
                    Console.WriteLine("/meta/connect");
                    resArr = JsonConvert.DeserializeObject<DxfeedMetaConnectRes[]>(response);
                    if (resArr.Length != 1)
                        throw new Exception("Not expecting multiple elements in the array");
                    finalRes = resArr[0];
                    break;
                case DxfeedChannel.SERVICEDATA:
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

            // match conversation (request/response) parts by ID if we can
            if (finalRes.Id != null)
            {
                int id;

                if (Int32.TryParse(finalRes.Id, out id))
                {
                    if (DxfeedConversations.ContainsKey(id))
                    {
                        DxfeedConversations[id].Received = DateTime.UtcNow;
                        DxfeedConversations[id].Response = finalRes;
                        HandleConversation(id);
                    }
                }
            }
            Console.WriteLine("Received: \r\n" + response);
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

