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

using System.Runtime.Serialization;

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
        OPEN,
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

