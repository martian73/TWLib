using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWLib.Models
{
    public enum OptionType
    {
        PUT = 'P',
        CALL = 'C'
    }

    public class Option : Security
    {
        public string Ticker { get; set; }
        public DateTime Expiry { get; set; }
        public decimal Strike { get; set; }
        public OptionType OptionType { get; set; }
        public UnderlyingType UnderlyingType { get; set; }

    }
}
