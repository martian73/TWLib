﻿// Generated by Xamasoft JSON Class Generator
// http://www.xamasoft.com/json-class-generator

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace TWLib.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OrderType
    {
        [EnumMember(Value = "Market")]
        MARKET,
        [EnumMember(Value = "Limit")]
        LIMIT,
        [EnumMember(Value = "Stop")]
        STOPMARKET,
        [EnumMember(Value = "Stop Limit")]
        STOPLIMIT
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum TimeInForce
    {
        [EnumMember(Value = "Day")]
        DAY,
        [EnumMember(Value = "GTC")]
        GTC,
        [EnumMember(Value = "GTD")]
        GTD,
        [EnumMember(Value = "Ext")]
        EXT
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum Action
    {
        [EnumMember(Value = "Buy to Open")]
        BUYTOOPEN,
        [EnumMember(Value = "Buy to Close")]
        BUYTOCLOSE,
        [EnumMember(Value = "Sell to Close")]
        SELLTOCLOSE,
        [EnumMember(Value = "Sell to Open")]
        SELLTOOPEN
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum PriceEffect
    {
        [EnumMember(Value = "Debit")]
        DEBIT,
        [EnumMember(Value = "Credit")]
        CREDIT
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum InstrumentType
    {
        [EnumMember(Value = "Equity")]
        EQUITY,
        [EnumMember(Value = "Future")]
        FUTURE
    }


    public class Order
    {
        public class Leg
        {
            [JsonProperty("instrument-type")]
            public InstrumentType InstrumentType { get; set; }

            [JsonProperty("symbol")]
            public string Symbol { get; set; }

            [JsonProperty("quantity")]
            public double Quantity { get; set; }

            [JsonProperty("action")]
            public Action Action { get; set; }
        }

        [JsonProperty("order-type")]
        public OrderType OrderType { get; set; }

        [JsonProperty("time-in-force")]
        public TimeInForce TimeInForce { get; set; }

        [JsonProperty("price")]
        public string Price { get; set; }

        [JsonProperty("price-effect")]
        public PriceEffect PriceEffect { get; set; }

        [JsonProperty("stop-trigger")]
        public string StopTrigger { get; set; }

        [JsonProperty("legs")]
        public IList<Leg> Legs { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }
    }
}
