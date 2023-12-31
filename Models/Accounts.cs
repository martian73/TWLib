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
    public class Accounts
    {
        public class Account2
        {
            [JsonProperty("account-number")]
            public string AccountNumber
            {
                get;
                set;
            }
            [JsonProperty("external-id")]
            public string ExternalId
            {
                get;
                set;
            }
            [JsonProperty("opened-at")]
            public string OpenedAt
            {
                get;
                set;
            }
            [JsonProperty("nickname")]
            public string Nickname
            {
                get;
                set;
            }
            [JsonProperty("account-type-name")]
            public string AccountTypeName
            {
                get;
                set;
            }
            [JsonProperty("day-trader-status")]
            public bool DayTraderStatus
            {
                get;
                set;
            }
            [JsonProperty("is-firm-error")]
            public bool IsFirmError
            {
                get;
                set;
            }
            [JsonProperty("is-firm-proprietary")]
            public bool IsFirmProprietary
            {
                get;
                set;
            }
            [JsonProperty("is-test-drive")]
            public bool IsTestDrive
            {
                get;
                set;
            }
            [JsonProperty("margin-or-cash")]
            public string MarginOrCash
            {
                get;
                set;
            }
            [JsonProperty("is-foreign")]
            public bool IsForeign
            {
                get;
                set;
            }
            [JsonProperty("investment-objective")]
            public string InvestmentObjective
            {
                get;
                set;
            }
            [JsonProperty("futures-account-purpose")]
            public string FuturesAccountPurpose
            {
                get;
                set;
            }
        }
        public class Item
        {
            [JsonProperty("account")]
            public Account2 Account
            {
                get;
                set;
            }
            [JsonProperty("authority-level")]
            public string AuthorityLevel
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
