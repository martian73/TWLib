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
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

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
        [EnumMember(Value = "Equity Option")]
        EQUITYOPTION,
        [EnumMember(Value = "Future")]
        FUTURE,
        [EnumMember(Value = "Index")]
        INDEX,
        [EnumMember(Value = "Unknown")]
        UNKNOWN
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum QuantityDirection
    {
        [EnumMember(Value = "Long")]
        LONG,
        [EnumMember(Value = "Short")]
        SHORT
    }

    public class Order
    {


        public static Order GetLimitOrder(string symbol, InstrumentType instrumentType, double price, int quantity, PriceEffect priceEffect, TWLib.Models.Action action, TimeInForce tif = TimeInForce.DAY)
        {
            Order order = new Order();
            order.OrderType = OrderType.LIMIT;
            order.Price = price.ToString("0.00");

            switch (action)
            {
                case TWLib.Models.Action.BUYTOOPEN:
                case TWLib.Models.Action.BUYTOCLOSE:
                    order.PriceEffect = PriceEffect.DEBIT;
                    break;
                default:
                    order.PriceEffect = PriceEffect.CREDIT;
                    break;
            }
            order.Source = "android";
            order.TimeInForce = tif;
            order.Legs = new List<Order.Leg>();
            Order.Leg leg = new Order.Leg();
            leg.Action = action;
            leg.InstrumentType = instrumentType;
            leg.Symbol = symbol;
            leg.Quantity = quantity;
            order.Legs.Add(leg);

            return order;
        }

        public static Order GetMarketOrder(string symbol, InstrumentType instrumentType, int quantity, TWLib.Models.Action action, TimeInForce tif = TimeInForce.DAY)
        {
            Order order = new Order();
            order.OrderType = OrderType.MARKET;
            order.Price = "";

            switch (action)
            {
                case TWLib.Models.Action.BUYTOOPEN:
                case TWLib.Models.Action.BUYTOCLOSE:
                    order.PriceEffect = PriceEffect.DEBIT;
                    break;
                default:
                    order.PriceEffect = PriceEffect.CREDIT;
                    break;
            }

            order.Source = "android";
            order.TimeInForce = tif;
            order.Legs = new List<Order.Leg>();
            Order.Leg leg = new Order.Leg();
            leg.Action = action;
            leg.InstrumentType = instrumentType;
            leg.Symbol = symbol;
            leg.Quantity = quantity;
            order.Legs.Add(leg);
            return order;
        }

        public static OrderStop GetStopMarketOrder(string symbol, InstrumentType instrumentType, double stopTrigger, int quantity, TWLib.Models.Action action, TimeInForce tif = TimeInForce.DAY)
        {

            if (action == TWLib.Models.Action.BUYTOOPEN || action == TWLib.Models.Action.SELLTOOPEN)
                throw new Exception("Invalid order.  Unable to initiate a possition with a stop order.");

            int contracts = quantity;

            OrderStop order = new OrderStop();
            order.OrderType = OrderType.STOPMARKET;
            order.StopTrigger = stopTrigger.ToString("0.00");
            order.Source = "android";
            order.TimeInForce = tif;
            order.Legs = new List<OrderStop.Leg>();
            OrderStop.Leg leg = new OrderStop.Leg();

            leg.Action = action;

            leg.InstrumentType = instrumentType;
            leg.Symbol = symbol;
            leg.Quantity = contracts;
            order.Legs.Add(leg);

            return order;
        }

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
        [JsonProperty("price")]
        public string Price
        {
            get;
            set;
        }
        [JsonProperty("price-effect")]
        public PriceEffect PriceEffect
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
