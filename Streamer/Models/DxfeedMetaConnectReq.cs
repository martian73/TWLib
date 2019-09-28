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
// Generated by Xamasoft JSON Class Generator
// http://www.xamasoft.com/json-class-generator

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TWLib.Streamer.Models
{
    public class DxfeedMetaConnectReq : DxfeedRequest
    {
        public DxfeedMetaConnectReq()
        {

        }

        public DxfeedMetaConnectReq(string clientId, int timeout)
        {
            Advice = new Advice2();
            Advice.Timeout = timeout;
            ClientId = clientId;
        }

        public class Advice2
        {

            [JsonProperty("timeout")]
            public int Timeout { get; set; }
        }

        [JsonProperty("clientId")]
        public string ClientId { get; set; }

        [JsonProperty("channel")]
        [JsonConverter(typeof(StringEnumConverter))]
        public override DxfeedChannel Channel { get { return DxfeedChannel.METACONNECT; } }

        [JsonProperty("connectionType")]
        public string ConnectionType { get { return "websocket"; } }

        [JsonProperty("advice")]
        public Advice2 Advice { get; set; }

        public override string Serialize()
        {
            DxfeedMetaConnectReq[] arr = new DxfeedMetaConnectReq[] { this };
            return JsonConvert.SerializeObject(arr);
        }

        public override TWRequest Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<DxfeedMetaHandshakeReq>(json);
        }
    }
}

