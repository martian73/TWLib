using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TWLib.Streamer.Models
{
    public abstract class TWResponse
    {
        [JsonIgnoreAttribute]
        public abstract StreamType StreamType { get; }

        public abstract string Serialize();
        public abstract TWResponse Deserialize(string json);
    }
}
