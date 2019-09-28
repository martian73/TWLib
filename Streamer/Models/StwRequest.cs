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
using Newtonsoft.Json.Converters;

namespace TWLib.Streamer.Models
{
    public class StwRequest : TWRequest
    {
        public StwRequest()
        {

        }

        public StwRequest(string authToken, StwAction action)
        {
            AuthToken = authToken;
            Action = action;
        }

        [JsonIgnoreAttribute]
        public override StreamType StreamType { get { return StreamType.STWSTREAMER; } }

        [JsonProperty("request-id")]
        public string Id { get; set; }

        [JsonProperty("auth-token")]
        public string AuthToken { get; set; }

        [JsonProperty("action")]
        [JsonConverter(typeof(StringEnumConverter))]
        public virtual StwAction Action { get; set; }

        [JsonProperty("source")]
        public string Source { get { return "android"; } }

        public override string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

        public override TWRequest Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<StwRequest>(json);
        }
    }
}




