﻿// Generated by Xamasoft JSON Class Generator
// http://www.xamasoft.com/json-class-generator

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TWLib.Models
{
    public class InstrumentsFutures
    {

        public class FutureEtfEquivalent
        {

            [JsonProperty("symbol")]
            public string Symbol { get; set; }

            [JsonProperty("share-quantity")]
            public int ShareQuantity { get; set; }
        }

        public class TickSize
        {

            [JsonProperty("value")]
            public string Value { get; set; }
        }

        public class OptionTickSize
        {

            [JsonProperty("value")]
            public string Value { get; set; }

            [JsonProperty("threshold")]
            public string Threshold { get; set; }
        }

        public class Item
        {

            [JsonProperty("symbol")]
            public string Symbol { get; set; }

            [JsonProperty("product-code")]
            public string ProductCode { get; set; }

            [JsonProperty("contract-size")]
            public string ContractSize { get; set; }

            [JsonProperty("tick-size")]
            public string TickSize { get; set; }

            [JsonProperty("notional-multiplier")]
            public string NotionalMultiplier { get; set; }

            [JsonProperty("main-fraction")]
            public string MainFraction { get; set; }

            [JsonProperty("sub-fraction")]
            public string SubFraction { get; set; }

            [JsonProperty("display-factor")]
            public string DisplayFactor { get; set; }

            [JsonProperty("last-trade-date")]
            public string LastTradeDate { get; set; }

            [JsonProperty("expiration-date")]
            public string ExpirationDate { get; set; }

            [JsonProperty("closing-only-date")]
            public string ClosingOnlyDate { get; set; }

            [JsonProperty("product-group")]
            public string ProductGroup { get; set; }

            [JsonProperty("active")]
            public bool Active { get; set; }

            [JsonProperty("active-month")]
            public bool ActiveMonth { get; set; }

            [JsonProperty("is-closing-only")]
            public bool IsClosingOnly { get; set; }

            [JsonProperty("future-etf-equivalent")]
            public FutureEtfEquivalent FutureEtfEquivalent { get; set; }

            [JsonProperty("exchange")]
            public string Exchange { get; set; }

            [JsonProperty("tick-sizes")]
            public IList<TickSize> TickSizes { get; set; }

            [JsonProperty("option-tick-sizes")]
            public IList<OptionTickSize> OptionTickSizes { get; set; }

            [JsonProperty("streamer-exchange-code")]
            public string StreamerExchangeCode { get; set; }

            [JsonProperty("first-notice-date")]
            public string FirstNoticeDate { get; set; }
        }

        public class Data2
        {

            [JsonProperty("items")]
            public IList<Item> Items { get; set; }
        }


        [JsonProperty("data")]
        public Data2 Data { get; set; }

        [JsonProperty("context")]
        public string Context { get; set; }

    }
}
