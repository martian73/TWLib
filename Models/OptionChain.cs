/*   This file is part of TWLib.
 *
 *    TWLib is free software: you can redistribute it and/or modify
 *    it under the terms of the GNU General Public License as published by
 *    the Free Software Foundation, either version 3 of the License, or
 *    (at your option) any later version.
 *
 *    TWLib is distributed in the hope that it will be useful,
 *    but WITHOUT ANY WARRANTY; without even the implied warranty of
 *    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 *    GNU General Public License for more details.
 *
 *    You should have received a copy of the GNU General Public License
 *    along with TWLib.  If not, see <https://www.gnu.org/licenses/>.
 ******************************************************************************
 *
 *    Project available from here: https://github.com/martian73/TWLib.git
 ******************************************************************************
 */
// Generated by Xamasoft JSON Class Generator
// http://www.xamasoft.com/json-class-generator

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace TWLib.Models
{
    public enum ExpirationType {[EnumMember(Value = "Regular")] REGULAR, [EnumMember(Value = "Weekly")] WEEKLY, [EnumMember(Value = "Quarterly")] QUARTERLY }
    public enum SettlementType {[EnumMember(Value = "PM")] PM }
    public class OptionChains
    {
        public class TickSize
        {
            [JsonProperty("value")]
            public string Value
            {
                get;
                set;
            }
        }
        public class Deliverable
        {
            [JsonProperty("id")]
            public int Id
            {
                get;
                set;
            }
            [JsonProperty("root-symbol")]
            public string RootSymbol
            {
                get;
                set;
            }
            [JsonProperty("deliverable-type")]
            public string DeliverableType
            {
                get;
                set;
            }
            [JsonProperty("description")]
            public string Description
            {
                get;
                set;
            }
            [JsonProperty("amount")]
            public string Amount
            {
                get;
                set;
            }
            [JsonProperty("symbol")]
            public string Symbol
            {
                get;
                set;
            }

            public static string GetStreamerSymbol(string chainSymbol)
            {
                //.SPY191007C293
                //SPY   191004C00185000

                string retval = null;
                Regex regex = new Regex(@"^(?<symbol>\S+)\s+(?<year>\d\d)(?<month>\d\d)(?<day>\d\d)(?<type>C|P)(?<strike>[0-9]{5})(?<strikethou>[0-9]{3})$");
                Match match = regex.Match(chainSymbol);

                double strikeThou = 0.0;
                double strike = 0.0;

                if (match.Success)
                {
                    if (double.TryParse(match.Groups["strike"].Value.ToString(), out strike) &&
                        double.TryParse(match.Groups["strikethou"].Value.ToString(), out strikeThou))
                    {
                        retval = String.Format(".{0}{1}{2}{3}{4}{5}{6}",
                            match.Groups["symbol"].Value.ToString(),
                            match.Groups["year"].Value.ToString(),
                            match.Groups["month"].Value.ToString(),
                            match.Groups["day"].Value.ToString(),
                            match.Groups["type"].Value.ToString(),
                            strike.ToString().TrimStart('0'),
                            strikeThou == 0.0 ? "" : "." + strikeThou.ToString().TrimEnd('0'));
                    }
                }

                return retval;
            }

            [JsonProperty("percent")]
            public string Percent
            {
                get;
                set;
            }
        }
        public class Strike
        {
            [JsonProperty("strike-price")]
            public string StrikePrice
            {
                get;
                set;
            }
            [JsonProperty("call")]
            public string Call
            {
                get;
                set;
            }
            [JsonProperty("put")]
            public string Put
            {
                get;
                set;
            }
        }
        public class Expiration
        {
            [JsonProperty("expiration-type")]
            public ExpirationType ExpirationType
            {
                get;
                set;
            }
            [JsonProperty("expiration-date")]
            public string ExpirationDate
            {
                get;
                set;
            }
            [JsonProperty("days-to-expiration")]
            public int DaysToExpiration
            {
                get;
                set;
            }
            [JsonProperty("settlement-type")]
            public SettlementType SettlementType
            {
                get;
                set;
            }
            [JsonProperty("strikes")]
            public IList<Strike> Strikes
            {
                get;
                set;
            }
        }
        public class Item
        {
            [JsonProperty("underlying-symbol")]
            public string UnderlyingSymbol
            {
                get;
                set;
            }
            [JsonProperty("root-symbol")]
            public string RootSymbol
            {
                get;
                set;
            }
            [JsonProperty("option-chain-type")]
            public string OptionChainType
            {
                get;
                set;
            }
            [JsonProperty("shares-per-contract")]
            public int SharesPerContract
            {
                get;
                set;
            }
            [JsonProperty("tick-sizes")]
            public IList<TickSize> TickSizes
            {
                get;
                set;
            }
            [JsonProperty("deliverables")]
            public IList<Deliverable> Deliverables
            {
                get;
                set;
            }
            [JsonProperty("expirations")]
            public IList<Expiration> Expirations
            {
                get;
                set;
            }
        }
        public class Data2
        {
            [JsonProperty("items")]
            public IList<Item> Items
            {
                get;
                set;
            }
        }
        [JsonProperty("data")]
        public Data2 Data
        {
            get;
            set;
        }
        [JsonProperty("context")]
        public string Context
        {
            get;
            set;
        }
    }
}
