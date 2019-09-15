using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWLib.Streamer.Models
{
    public class StreamerRequest
    {
        [JsonProperty("request-id")]
        public string Id { get; set; }

        [JsonProperty("auth-token")]
        public string AuthToken { get; set; }

        [JsonProperty("action")]
        public Action Action { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }
    }
}
