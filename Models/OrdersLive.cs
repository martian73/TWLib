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
    public class OrdersLive
    {
        public class Fill
        {
            [JsonProperty("ext-group-fill-id")]
            public string ExtGroupFillId
            {
                get;
                set;
            }
            [JsonProperty("ext-exec-id")]
            public string ExtExecId
            {
                get;
                set;
            }
            [JsonProperty("quantity")]
            public int Quantity
            {
                get;
                set;
            }
            [JsonProperty("fill-price")]
            public string FillPrice
            {
                get;
                set;
            }
            [JsonProperty("filled-at")]
            public string FilledAt
            {
                get;
                set;
            }
        }
        public class Leg
        {
            [JsonProperty("instrument-type")]
            public string InstrumentType
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
            public int Quantity
            {
                get;
                set;
            }
            [JsonProperty("remaining-quantity")]
            public int RemainingQuantity
            {
                get;
                set;
            }
            [JsonProperty("action")]
            public string Action
            {
                get;
                set;
            }
            [JsonProperty("fills")]
            public IList<Fill> Fills
            {
                get;
                set;
            }
        }
        public class Item
        {
            [JsonProperty("id")]
            public int Id
            {
                get;
                set;
            }
            [JsonProperty("account-number")]
            public string AccountNumber
            {
                get;
                set;
            }
            [JsonProperty("time-in-force")]
            public string TimeInForce
            {
                get;
                set;
            }
            [JsonProperty("order-type")]
            public string OrderType
            {
                get;
                set;
            }
            [JsonProperty("size")]
            public int Size
            {
                get;
                set;
            }
            [JsonProperty("underlying-symbol")]
            public string UnderlyingSymbol
            {
                get;
                set;
            }
            [JsonProperty("price")]
            public string Price
            {
                get;
                set;
            }
            [JsonProperty("price-effect")]
            public string PriceEffect
            {
                get;
                set;
            }
            [JsonProperty("status")]
            public string Status
            {
                get;
                set;
            }
            [JsonProperty("cancellable")]
            public bool Cancellable
            {
                get;
                set;
            }
            [JsonProperty("editable")]
            public bool Editable
            {
                get;
                set;
            }
            [JsonProperty("edited")]
            public bool Edited
            {
                get;
                set;
            }
            [JsonProperty("received-at")]
            public string ReceivedAt
            {
                get;
                set;
            }
            [JsonProperty("updated-at")]
            public object UpdatedAt
            {
                get;
                set;
            }
            [JsonProperty("reject-reason")]
            public string RejectReason
            {
                get;
                set;
            }
            [JsonProperty("terminal-at")]
            public string TerminalAt
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
        [JsonProperty("api-version")]
        public string ApiVersion
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
