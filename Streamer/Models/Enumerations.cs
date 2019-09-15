using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWLib.Streamer.Models
{
    public enum StreamerAction
    {
        [JsonProperty("heartbeat")]
        HEARTBEAT,
        [JsonProperty("account-subscribe")]
        ACCOUNTSUBSCRIBE,
        [JsonProperty("user-message-subscribe")]
        USERMESSAGESUBSRCIBE,
        [JsonProperty("public-watchlists-subscribe")]
        PUBLICWATCHLISTSSUBSCRIBE
    }

    public enum DxfeedChannel
    {
        [JsonProperty("/meta/handshake")]
        METAHANDSHAKE,
        [JsonProperty("/meta/connect")]
        METACONNECT,
        [JsonProperty("/service/sub")]
        SERVICESUB,
        [JsonProperty("/service/state")]
        SERVICESTATE,
        [JsonProperty("/service/data")]
        SERVICEDATA


    }
}
