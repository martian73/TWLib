using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWLib.Streamer.Models
{
    /// <summary>
    /// Base type that applies to all DxfeedRequests having a minimnum of an id and channel
    /// </summary>
    public class DxfeedRequest : TWRequest
    {
        [JsonIgnoreAttribute]
        public override StreamType StreamType { get { return StreamType.DXFEED; } }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("channel")]
        public virtual string Channel { get; set; }

        public override string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

        public override TWRequest Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<DxfeedRequest>(json);
        }
    }
}
