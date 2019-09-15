﻿// Generated by Xamasoft JSON Class Generator
// http://www.xamasoft.com/json-class-generator

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace TWLib.Models
{
    public class TradingStatus
    {
        public class Data2
        {

            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("account-number")]
            public string AccountNumber { get; set; }

            [JsonProperty("options-level")]
            public string OptionsLevel { get; set; }

            [JsonProperty("is-frozen")]
            public bool IsFrozen { get; set; }

            [JsonProperty("is-closing-only")]
            public bool IsClosingOnly { get; set; }

            [JsonProperty("is-in-margin-call")]
            public bool IsInMarginCall { get; set; }

            [JsonProperty("is-pattern-day-trader")]
            public bool IsPatternDayTrader { get; set; }

            [JsonProperty("is-in-day-trade-equity-maintenance-call")]
            public bool IsInDayTradeEquityMaintenanceCall { get; set; }

            [JsonProperty("equities-margin-calculation-type")]
            public string EquitiesMarginCalculationType { get; set; }

            [JsonProperty("has-intraday-equities-margin")]
            public bool HasIntradayEquitiesMargin { get; set; }

            [JsonProperty("is-closed")]
            public bool IsClosed { get; set; }

            [JsonProperty("is-futures-enabled")]
            public bool IsFuturesEnabled { get; set; }

            [JsonProperty("is-futures-intra-day-enabled")]
            public bool IsFuturesIntraDayEnabled { get; set; }

            [JsonProperty("futures-margin-rate-multiplier")]
            public string FuturesMarginRateMultiplier { get; set; }

            [JsonProperty("is-futures-closing-only")]
            public bool IsFuturesClosingOnly { get; set; }

            [JsonProperty("short-calls-enabled")]
            public bool ShortCallsEnabled { get; set; }

            [JsonProperty("is-full-equity-margin-required")]
            public bool IsFullEquityMarginRequired { get; set; }

            [JsonProperty("is-portfolio-margin-enabled")]
            public bool IsPortfolioMarginEnabled { get; set; }

            [JsonProperty("fee-schedule-name")]
            public string FeeScheduleName { get; set; }
        }


        [JsonProperty("data")]
        public Data2 Data { get; set; }

        [JsonProperty("api-version")]
        public string ApiVersion { get; set; }

        [JsonProperty("context")]
        public string Context { get; set; }

    }
}