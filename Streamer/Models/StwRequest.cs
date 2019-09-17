using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public string Source {  get { return "android"; } }

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



