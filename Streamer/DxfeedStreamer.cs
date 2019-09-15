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
    public class DxfeedStreamer : TWWebSocketManager
    {
        private string AuthToken;
        private DateTime LastValidate;
        private StreamerTokens StreamTokens;
       
        public DxfeedStreamer()
        {
            StreamActive = false;
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
            StreamerWebsocketUrl = StreamTokens.Data.StreamerUrl;
            StreamerApiUrl = StreamTokens.Data.WebsocketUrl;

            // Init Dxfeed
            // Start Dxfeed heartbeat thread

            // Init Streamer
            // Start streamer heartbeat thread

            // Init MetaQueue
            // Start MetaQueue heartbeat thread

        }

        public void InitDxfeed()
        {
            if (!StreamActive)
                throw new Exception("Not logged in");

            if (StreamActive)
                throw new Exception("DxFeed already active.");

           
        }

        public void Restart()
        {
            StreamActive = false;

            // Join queue threads before starting

            StreamTokens = GetQuoteStreamerTokens();
            StreamerApiUrl = StreamTokens.Data.StreamerUrl;
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
