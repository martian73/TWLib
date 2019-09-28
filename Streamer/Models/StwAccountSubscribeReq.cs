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
using System.Collections.Generic;

namespace TWLib.Streamer.Models
{
    public class StwAccountSubscribeReq : StwRequest
    {
        [JsonProperty("value")]
        public IList<string> Value { get; set; }

        public override TWRequest Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<StwAccountSubscribeReq>(json);
        }

        public override string Serialize()
        {
            StwAccountSubscribeReq[] arr = new StwAccountSubscribeReq[] { this };
            return JsonConvert.SerializeObject(arr);
        }
    }
}

