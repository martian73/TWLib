using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using TWLib.Streamer.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.WebSockets;
using System.Threading;

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
    
    public class DxfeedStreamer : TWWebSocketManager
    {
        public Dictionary <int, DxConvo> DxfeedConversations;

        private string AuthToken;
        private DateTime LastValidate;
        private StreamerTokens StreamTokens;
       
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
        }

        public void Init(string authToken)
        {
            if (StreamActive)
                return;

            AuthToken = authToken;

            StreamTokens = GetQuoteStreamerTokens();
            StreamerWebsocketUrl = (StreamTokens.Data.WebsocketUrl + "/cometd").Replace("https://", "wss://");
            StreamerApiUrl = StreamTokens.Data.StreamerUrl;


            HeartBeatThread = new Thread(() => { HeartBeatLoop(); });
            HeartBeatThread.Start();

            Start();

            DxfeedMetaHandshakeReq req = new DxfeedMetaHandshakeReq();
            req.Advice = new DxfeedMetaHandshakeReq.Advice2();
            req.Ext = new DxfeedMetaHandshakeReq.Ext2();

            req.Advice.Interval = 0;
            req.Advice.Timeout = 60000;
            req.Ext.ComDevexpertsAuthToken = StreamTokens.Data.Token;

            while (!StreamActive)
                Thread.Sleep(100);
            
            SendRequest(req);
        }

        public void Restart()
        {
            StreamActive = false;

            // Wait for heartbeat thread to exit
            HeartBeatThread.Join();
            HeartBeatThread = null;

            Init(AuthToken);
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
                    resArr = JsonConvert.DeserializeObject<DxFeedMetaHandshakeRes[]>(response);
                    if (resArr.Length != 1)
                        throw new Exception("Not expecting multiple elements in the array");
                    res = resArr[0];
                    break;
                case DxfeedChannel.METACONNECT:
                    Console.WriteLine("/meta/connect");
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
                    }
                }
            }
            Console.WriteLine("Received: \r\n" + JsonConvert.SerializeObject(res));
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
