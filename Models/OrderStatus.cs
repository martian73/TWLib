using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TWLib.Models
{
    [DataContract]
    public class OrderStatus
    {
        [DataMember(Name = "underlying_symbol")]
        public string UnderlyingSymbol { get; set; }

        [DataMember(Name = "price")]
        public decimal Price { get; set; }

        [DataMember(Name = "price-effect")]
        public string PriceEffect { get; set; }

        [DataMember(Name = "order-type")]
        public string OrderType { get; set; }

        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "time-in-force")]
        public string TimeInForce { get; set; }
    }
}
