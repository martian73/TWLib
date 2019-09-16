using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWLib.Streamer.Models
{
    // channel is in every DXChannel Response
    public class DxfeedResponse : TWResponse
    {
        [JsonIgnoreAttribute]
        public override StreamType StreamType
        {
            get
            {
                return StreamType.DXFEED;
            }
        }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("channel")]
        [JsonConverter(typeof(StringEnumConverter))]
        public DxfeedChannel Channel { get; set; }

        public override string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

        public override TWResponse Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<DxfeedResponse>(json);
        }
    }
}
