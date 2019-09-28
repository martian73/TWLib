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
using System.Collections.Generic;

namespace TWLib.Models
{
    public class OrderStop
    {
        public class Leg
        {
            [JsonProperty("instrument-type")]
            public InstrumentType InstrumentType
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
            [JsonProperty("quantity")]
            public double Quantity
            {
                get;
                set;
            }
            [JsonProperty("action")]
            public Action Action
            {
                get;
                set;
            }
        }
        [JsonProperty("order-type")]
        public OrderType OrderType
        {
            get;
            set;
        }
        [JsonProperty("time-in-force")]
        public TimeInForce TimeInForce
        {
            get;
            set;
        }
        [JsonProperty("stop-trigger")]
        public string StopTrigger
        {
            get;
            set;
        }
        [JsonProperty("legs")]
        public IList<Leg> Legs
        {
            get;
            set;
        }
        [JsonProperty("source")]
        public string Source
        {
            get;
            set;
        }
    }
}
