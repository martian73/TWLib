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
    public class Balances
    {
        public class Data2
        {
            [JsonProperty("account-number")]
            public string AccountNumber
            {
                get;
                set;
            }
            [JsonProperty("cash-balance")]
            public string CashBalance
            {
                get;
                set;
            }
            [JsonProperty("long-equity-value")]
            public string LongEquityValue
            {
                get;
                set;
            }
            [JsonProperty("short-equity-value")]
            public string ShortEquityValue
            {
                get;
                set;
            }
            [JsonProperty("long-derivative-value")]
            public string LongDerivativeValue
            {
                get;
                set;
            }
            [JsonProperty("short-derivative-value")]
            public string ShortDerivativeValue
            {
                get;
                set;
            }
            [JsonProperty("long-futures-value")]
            public string LongFuturesValue
            {
                get;
                set;
            }
            [JsonProperty("short-futures-value")]
            public string ShortFuturesValue
            {
                get;
                set;
            }
            [JsonProperty("long-futures-derivative-value")]
            public string LongFuturesDerivativeValue
            {
                get;
                set;
            }
            [JsonProperty("short-futures-derivative-value")]
            public string ShortFuturesDerivativeValue
            {
                get;
                set;
            }
            [JsonProperty("debit-margin-balance")]
            public string DebitMarginBalance
            {
                get;
                set;
            }
            [JsonProperty("long-margineable-value")]
            public string LongMargineableValue
            {
                get;
                set;
            }
            [JsonProperty("short-margineable-value")]
            public string ShortMargineableValue
            {
                get;
                set;
            }
            [JsonProperty("margin-equity")]
            public string MarginEquity
            {
                get;
                set;
            }
            [JsonProperty("equity-buying-power")]
            public string EquityBuyingPower
            {
                get;
                set;
            }
            [JsonProperty("derivative-buying-power")]
            public string DerivativeBuyingPower
            {
                get;
                set;
            }
            [JsonProperty("day-trading-buying-power")]
            public string DayTradingBuyingPower
            {
                get;
                set;
            }
            [JsonProperty("futures-margin-requirement")]
            public string FuturesMarginRequirement
            {
                get;
                set;
            }
            [JsonProperty("available-trading-funds")]
            public string AvailableTradingFunds
            {
                get;
                set;
            }
            [JsonProperty("maintenance-requirement")]
            public string MaintenanceRequirement
            {
                get;
                set;
            }
            [JsonProperty("maintenance-call-value")]
            public string MaintenanceCallValue
            {
                get;
                set;
            }
            [JsonProperty("reg-t-call-value")]
            public string RegTCallValue
            {
                get;
                set;
            }
            [JsonProperty("day-trading-call-value")]
            public string DayTradingCallValue
            {
                get;
                set;
            }
            [JsonProperty("day-equity-call-value")]
            public string DayEquityCallValue
            {
                get;
                set;
            }
            [JsonProperty("net-liquidating-value")]
            public string NetLiquidatingValue
            {
                get;
                set;
            }
            [JsonProperty("cash-available-to-withdraw")]
            public string CashAvailableToWithdraw
            {
                get;
                set;
            }
            [JsonProperty("day-trade-excess")]
            public string DayTradeExcess
            {
                get;
                set;
            }
            [JsonProperty("pending-cash")]
            public string PendingCash
            {
                get;
                set;
            }
            [JsonProperty("pending-cash-effect")]
            public string PendingCashEffect
            {
                get;
                set;
            }
            [JsonProperty("snapshot-date")]
            public string SnapshotDate
            {
                get;
                set;
            }
            [JsonProperty("reg-t-margin-requirement")]
            public string RegTMarginRequirement
            {
                get;
                set;
            }
            [JsonProperty("futures-overnight-margin-requirement")]
            public string FuturesOvernightMarginRequirement
            {
                get;
                set;
            }
            [JsonProperty("futures-intraday-margin-requirement")]
            public string FuturesIntradayMarginRequirement
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
