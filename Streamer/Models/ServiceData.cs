using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Runtime.Serialization;
using TWLib.Models;

namespace TWLib.Streamer.Models
{
    public enum ServiceDataType : int
    {
        [EnumMember(Value = "Quote")]
        QUOTE = 1,
        [EnumMember(Value = "Greeks")]
        GREEKS = 2,
        [EnumMember(Value = "Trade")]
        TRADE = 4,
        [EnumMember(Value = "Summary")]
        SUMMARY = 8,
        [EnumMember(Value = "TheoPrice")]
        THEOPRICE = 16,
        [EnumMember(Value = "Profile")]
        PROFILE = 32,
        [EnumMember(Value = "order")]
        ORDER = 64
    }

    public enum TickDirection
    {
        [EnumMember(Value = "ZERO_DOWN")]
        ZERO_DOWN,
        [EnumMember(Value = "ZERO_UP")]
        ZERO_UP,
        [EnumMember(Value = "UNDEFINED")]
        UNDEFINED
    }

    public enum PriceType
    {
        [EnumMember(Value = "REGULAR")]
        REGULAR,
        [EnumMember(Value = "FINAL")]
        FINAL
    }

    // Greeks
    //["eventSymbol", "eventTime", "eventFlags", "index", "time", "sequence", "price", "volatility", "delta", "gamma", "theta", "rho", "vega"]],
    //[".XLF191018C24",0          ,0           ,6746789276744155136,1570859289923,0,         3.723814,0.6500939   ,0.9376462,0.04665602,-0.02063665,0.00475716,0.00496853]]

    // Quote
    //["eventSymbol", "eventTime", "sequence", "timeNanoPart", "bidTime", "bidExchangeCode", "bidPrice", "bidSize", "askTime", "askExchangeCode", "askPrice", "askSize"]],
    //[".XLF191018C24",0,           0,         0,            1570824848000,"Z",               3.35,      11.0,     1570824881000,"B",               4.1,       60.0]]

    // Trade
    //["eventSymbol", "eventTime", "time", "timeNanoPart", "sequence", "exchangeCode", "price", "size", "dayVolume", "dayTurnover", "tickDirection", "extendedTradingHours"]],
    //[".XLF191018C24",0,          1570650209000,0,            0,         "Q",            3.15,   12.0,   0.0,      "NaN",        "ZERO_DOWN",    false

    // Summary

    //["eventSymbol","eventTime","dayId","dayOpenPrice","dayHighPrice","dayLowPrice","dayClosePrice","dayClosePriceType","prevDayId","prevDayClosePrice","prevDayClosePriceType","prevDayVolume","openInterest"]],
    //[".XLF191018C24",0,         18180,  "NaN",         "NaN",        "NaN",         "NaN",        "REGULAR",            18178,               3.15,      "REGULAR",             "NaN",                 12]],

    // TheoPrice

    //["eventSymbol", "eventTime", "eventFlags", "index",            "time",       "sequence", "price",         "underlyingPrice",   "delta",               "gamma",      "dividend", "interest"]],
    //[".XLF191018C24",0,          0,            6746789276744155136,1570859289923,0,          3.7238136159995,   27.65,             0.937646506000881, 0.0466559249878944,  0.0,       0.0]]


    // Profile

    //["eventSymbol","eventTime","description",           "shortSaleRestriction","tradingStatus","statusReason","haltStartTime","haltEndTime","highLimitPrice","lowLimitPrice"]],
    //["SPY",        0,          "SPDR S&P 500 ETF Trust","INACTIVE",             "UNDEFINED",    null,          0,              0,            0.0,             0.0,
    //"MSFT",0,"Microsoft Corp","INACTIVE","ACTIVE",null,0,0,0.0,0.0,"AAPL",0,"Apple Inc","INACTIVE","ACTIVE",null,1546464305000,1546465800000,0.0,0.0]]

    public abstract class ServiceData
    {
        [JsonIgnoreAttribute]
        public InstrumentType InstrumentType
        {
            get
            {
                if (EventSymbol == null || EventSymbol.Length < 1)
                {
                    return InstrumentType.UNKNOWN;
                }
                if (EventSymbol.StartsWith("."))
                {
                    return InstrumentType.EQUITYOPTION;
                }
                if (EventSymbol.StartsWith("/"))
                {
                    return InstrumentType.FUTURE;
                }
                if (EventSymbol.StartsWith("$"))
                {
                    return InstrumentType.INDEX;
                }
                return InstrumentType.EQUITY;
            }
        }

        [JsonIgnoreAttribute]
        public abstract ServiceDataType DataType { get; }

        [JsonProperty("eventSymbol")]
        public string EventSymbol { get; set; }

        [JsonProperty("eventTime")]
        public long EventTime { get; set; }

        public void DumpProperties()
        {
            var type = this.GetType();
            foreach (var prop in type.GetProperties())
            {
                var val = prop.GetValue(this, new object[] { });
                var valStr = val == null ? "" : val.ToString();
                Console.Write(prop.Name + ": " + valStr + " ");
            }
            Console.WriteLine();
        }
    }

    public class Quote : ServiceData
    {
        [JsonIgnoreAttribute]
        public static string[] Headers = null;

        [JsonIgnoreAttribute]
        public override ServiceDataType DataType { get { return ServiceDataType.QUOTE; } }

        [JsonProperty("sequence")]
        public int Sequence { get; set; }

        [JsonProperty("timeNanoPart")]
        public int TimeNanoPart { get; set; }

        [JsonProperty("bidTime")]
        public double BidTime { get; set; }

        [JsonProperty("bidExchangeCode")]
        public string BidExchangeCode { get; set; }

        [JsonProperty("bidPrice")]
        public double BidPrice { get; set; }

        [JsonProperty("bidSize")]
        public double BidSize { get; set; }

        [JsonProperty("askTime")]
        public double AskTime { get; set; }

        [JsonProperty("askExchangeCode")]
        public string AskExchangeCode { get; set; }

        [JsonProperty("askPrice")]
        public double AskPrice { get; set; }

        [JsonProperty("askSize")]
        public double AskSize { get; set; }
    }

    public class Trade : ServiceData
    {
        [JsonIgnoreAttribute]
        public static string[] Headers = null;

        [JsonIgnoreAttribute]
        public override ServiceDataType DataType { get { return ServiceDataType.TRADE; } }

        [JsonProperty("time")]
        public double Time { get; set; }

        [JsonProperty("timeNanoPart")]
        public int TimeNanoPart { get; set; }

        [JsonProperty("sequence")]
        public int Sequence { get; set; }

        [JsonProperty("exchangeCode")]
        public string ExchangeCode { get; set; }

        [JsonProperty("price")]
        public string Price { get; set; }

        [JsonProperty("size")]
        public double Size { get; set; }

        [JsonProperty("dayVolume")]
        public double DayVolume { get; set; }

        [JsonProperty("dayTurnover")]
        public double? DayTurnover { get; set; }

        [JsonProperty("tickDirection")]
        [JsonConverter(typeof(StringEnumConverter))]
        public TickDirection TickDirection { get; set; }

        [JsonProperty("extendedTradingHours")]
        public bool ExtendedTradingHours { get; set; }
    }

    public class Greeks : ServiceData
    {
        [JsonIgnoreAttribute]
        public static string[] Headers = null;

        [JsonIgnoreAttribute]
        public override ServiceDataType DataType { get { return ServiceDataType.GREEKS; } }

        [JsonProperty("eventFlags")]
        public int EventFlags { get; set; }

        [JsonProperty("index")]
        public long Index { get; set; }

        [JsonProperty("time")]
        public double Time { get; set; }

        [JsonProperty("sequence")]
        public int Sequence { get; set; }

        [JsonProperty("price")]
        public double Price { get; set; }

        [JsonProperty("volatility")]
        public double Volatility { get; set; }

        [JsonProperty("delta")]
        public double Delta { get; set; }

        [JsonProperty("gamma")]
        public double Gamma { get; set; }

        [JsonProperty("theta")]
        public double Theta { get; set; }

        [JsonProperty("rho")]
        public double Rho { get; set; }

        [JsonProperty("vega")]
        public double Vega { get; set; }
    }

    public class Summary : ServiceData
    {
        [JsonIgnoreAttribute]
        public static string[] Headers = null;

        [JsonIgnoreAttribute]
        public override ServiceDataType DataType { get { return ServiceDataType.SUMMARY; } }

        [JsonProperty("dayId")]
        public int DayId { get; set; }

        [JsonProperty("dayOpenPrice")]
        public double? DayOpenPrice { get; set; }

        [JsonProperty("dayHighPrice")]
        public double? DayHighPrice { get; set; }

        [JsonProperty("dayLowPrice")]
        public double? DayLowPrice { get; set; }

        [JsonProperty("dayClosePrice")]
        public double? DayClosePrice { get; set; }

        [JsonProperty("dayClosePriceType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public PriceType DayClosePriceType { get; set; }

        [JsonProperty("prevDayId")]
        public int PrevDayId { get; set; }

        [JsonProperty("prevDayClosePrice")]
        public double PrevDayClosePrice { get; set; }

        [JsonProperty("prevDayClosePriceType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public PriceType PrevDayClosePriceType { get; set; }

        [JsonProperty("prevDayVolume")]
        public double? PrevDayVolume { get; set; }

        [JsonProperty("openInterest")]
        public long OpenInterest { get; set; }
    }

    public class TheoPrice : ServiceData
    {
        [JsonIgnoreAttribute]
        public static string[] Headers = null;

        [JsonIgnoreAttribute]
        public override ServiceDataType DataType { get { return ServiceDataType.THEOPRICE; } }

        [JsonProperty("eventFlags")]
        public int EventFlags { get; set; }

        [JsonProperty("index")]
        public long Index { get; set; }

        [JsonProperty("time")]
        public double Time { get; set; }

        [JsonProperty("sequence")]
        public int Sequence { get; set; }

        [JsonProperty("price")]
        public double Price { get; set; }

        [JsonProperty("underlyingPrice")]
        public double UnderlyingPrice { get; set; }

        [JsonProperty("delta")]
        public double Delta { get; set; }

        [JsonProperty("gamma")]
        public double Gamma { get; set; }

        [JsonProperty("dividend")]
        public double Dividend { get; set; }

        [JsonProperty("interest")]
        public double Interest { get; set; }
    }

    public class Profile : ServiceData
    {
        [JsonIgnoreAttribute]
        public static string[] Headers = null;

        [JsonIgnoreAttribute]
        public override ServiceDataType DataType { get { return ServiceDataType.PROFILE; } }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("shortSaleRestriction")]
        public string ShortSaleRestriction { get; set; }

        [JsonProperty("tradingStatus")]
        public string TradingStatus { get; set; }

        [JsonProperty("statusReason")]
        public string StatusReason { get; set; }

        [JsonProperty("haltStartTime")]
        public long HaltStartTime { get; set; }

        [JsonProperty("haltEndTime")]
        public long HaltEndTime { get; set; }

        [JsonProperty("highLimitPrice")]
        public double HighLimitPrice { get; set; }

        [JsonProperty("lowLimitPrice")]
        public double LowLimitPrice { get; set; }
    }
}
