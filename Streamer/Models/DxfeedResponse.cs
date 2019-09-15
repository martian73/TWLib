using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWLib.Streamer.Models
{
    // channel and ID are in every DXChannel Response
    public class DxfeedResponse : TWResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("channel")]
        public string Channel { get; set; }

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
