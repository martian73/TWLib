using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TWLib.Streamer.Models
{

    public enum StreamType
    {
        DXFEED,
        STWSTREAMER,
        METAQUEUE
    }

    public enum DxFeedStreamState
    {
        NONE,
        HANDSHAKE,
        CONNECT,
        READY
    }

    public enum DxfeedChannel
    {
        [EnumMember(Value = "/meta/handshake")]
        METAHANDSHAKE,
        [EnumMember(Value = "/meta/connect")]
        METACONNECT,
        [EnumMember(Value = "/service/sub")]
        SERVICESUB,
        [EnumMember(Value = "/service/state")]
        SERVICESTATE,
        [EnumMember(Value = "/service/data")]
        SERVICEDATA,
        [EnumMember(Value = "/service/timeSeriesData")]
        SERVICETIMESERIESDATA
    }

    public enum StwAction
    {
        [EnumMember(Value = "heartbeat")]
        HEARTBEAT,
        [EnumMember(Value = "account-subscribe")]
        ACCOUNTSUBSCRIBE,
        [EnumMember(Value = "user-message-subscribe")]
        USERMESSAGESUBSRCIBE,
        [EnumMember(Value = "public-watchlists-subscribe")]
        PUBLICWATCHLISTSSUBSCRIBE
    }

}
