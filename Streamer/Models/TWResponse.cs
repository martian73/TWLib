using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWLib.Streamer.Models
{
    public abstract class TWResponse
    {
        public abstract string Serialize();
        public abstract TWResponse Deserialize(string json);
    }
}
