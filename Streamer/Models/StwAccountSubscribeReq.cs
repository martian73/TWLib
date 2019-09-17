using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
