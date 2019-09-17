using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TWLib.Streamer
{
    public class MetaqueStreamer : TWWebSocketManager, IDisposable
    {
        const int HeartBeatMs = 10000;
        const string Passcode = "xGne8HLEyQscqQa3";
        const string AcceptVersion = "1.1,1.0";
        const string APIUrl = "wss://metaque.tastyworks.com";
        int Subs = 0;

        bool Connected = false;

        public override void Init(string authToken)
        {
            StreamerWebsocketUrl = APIUrl;

            base.Start();

            while (!StreamActive)
                Thread.Sleep(100);
            
            SendConnect();

            while (!Connected)
                Thread.Sleep(100);

            HeartBeatThread = new Thread(() => { HeartBeatLoop(); });
            HeartBeatThread.Start();
        }
        
        private void SendConnect()
        {
            string str = String.Format(
                "CONNECT\nlogin:client\npasscode:{0}\naccept-version:{1}\nheart-beat:{2},{2}\n\n",
                Passcode, AcceptVersion, HeartBeatMs.ToString());
            List<byte> bytes = new List<byte>(Encoding.UTF8.GetBytes(str));
            bytes.Add(0);
            SendRawRequest(bytes.ToArray());
        }

        public void SubscribeMarketMetrics(string symbol)
        {
            string str = String.Format(
                "SUBSCRIBE\nid:sub-{0}\ndestination:/topic/market-metrics.{1}\n\n", Subs++, symbol);
            List<byte> bytes = new List<byte>(Encoding.UTF8.GetBytes(str));
            bytes.Add(0);
            SendRawRequest(bytes.ToArray());
        }

        public override void ReceiveResponse(string response)
        {
            if (!Connected && response != null && response.Contains("CONNECTED"))
            {
                Connected = true;
            }

            Console.WriteLine("Response MetaqueStreamer:\r\n" + response);
        }

        public override void Restart()
        {
            throw new NotImplementedException();
        }

        public override void Stop()
        {
            StreamActive = false;
            HeartBeatThread.Join();
            HeartBeatThread = null;
        }

        protected override void HeartBeatLoop()
        {
            string payload = "\n";
            SendRawRequest(payload);
            DateTime lastHeartbeat = DateTime.UtcNow;

            while (StreamActive)
            {
                if (lastHeartbeat.AddMilliseconds(HeartBeatMs) < DateTime.UtcNow)
                {
                    SendRawRequest(payload);
                    lastHeartbeat = DateTime.UtcNow;
                }
                Thread.Sleep(100);
            }
            Console.WriteLine("Exiting Metaque Streamer HeartBeatLoop");
        }
    }
}
