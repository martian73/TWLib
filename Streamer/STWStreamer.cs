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
using System.Collections.Generic;
using System.Threading;
using TWLib.Streamer.Models;

namespace TWLib.Streamer
{
    public class STWStreamer : TWWebSocketManager, IDisposable
    {
        private string AuthToken;

        public STWStreamer(string apiUrl = "wss://streamer.tastyworks.com")
        {
            StreamerWebsocketUrl = apiUrl;
        }

        public override void Init(string authToken)
        {
            if (StreamerWebsocketUrl == null || !StreamerWebsocketUrl.StartsWith("wss://"))
            {
                throw new Exception("Streamer WebsocketUrl not valid.");
            }

            AuthToken = authToken;

            base.Start();

            // Wait for websocket establishment
            while (!StreamActive)
                Thread.Sleep(100);

            // start sending heartbeat packets
            HeartBeatThread = new Thread(() =>
            {
                HeartBeatLoop();
            });

            HeartBeatThread.Start();
        }

        public void AccountSubscribe(List<string> accounts)
        {
            StwAccountSubscribeReq req = new StwAccountSubscribeReq();
            req.AuthToken = AuthToken;
            req.Action = StwAction.ACCOUNTSUBSCRIBE;
            req.Value = accounts;

            SendRequest(req);
        }

        public void UserMessageSubscribe()
        {
            StwRequest req = new StwRequest(AuthToken, StwAction.USERMESSAGESUBSRCIBE);

            SendRequest(req);
        }

        public void PublicWatchListSubscribe()
        {
            StwRequest req = new StwRequest(AuthToken, StwAction.PUBLICWATCHLISTSSUBSCRIBE);

            SendRequest(req);
        }

        public override void ReceiveResponse(string response)
        {
            Console.WriteLine("Response STWStreamer:\r\n" + response);
            //StwResponse sRes = JsonConvert.DeserializeObject<StwResponse>(response);
        }

        public void SendRequest(StwRequest request)
        {
            request.Id = GetNextRequestID().ToString();
            base.SendRequest(request);
        }

        public override void Stop()
        {
            StreamActive = false;
            HeartBeatThread.Join();
            HeartBeatThread = null;

            Console.WriteLine("Exiting Tasty Streamer.");
        }

        protected override void HeartBeatLoop()
        {
            DateTime lastSent = DateTime.UtcNow;

            StwRequest req = new StwRequest(AuthToken, StwAction.HEARTBEAT);
            SendRequest(req);

            while (StreamActive)
            {
                if (lastSent.AddSeconds(20) < DateTime.UtcNow)
                {
                    req = new StwRequest(AuthToken, StwAction.HEARTBEAT);
                    SendRequest(req);
                    lastSent = DateTime.UtcNow;
                }
                Thread.Sleep(500);
            }
            Console.WriteLine("Exiting STWStreamer HeartBeatLoop");
        }
    }
}

