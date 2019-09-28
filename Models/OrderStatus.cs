/*   This file is part of TWLib.
 *
 *    TWLib is free software: you can redistribute it and/or modify
 *    it under the terms of the GNU General Public License as published by
 *    the Free Software Foundation, either version 3 of the License, or
 *    (at your option) any later version.

 *    TWLib is distributed in the hope that it will be useful,
 *    but WITHOUT ANY WARRANTY;
 without even the implied warranty of
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
using System;
 using System.Collections.Generic;
 using System.Linq;
 using System.Runtime.Serialization;
 using System.Text;
 using System.Threading.Tasks;
 using Newtonsoft.Json;
  namespace TWLib.Models {     [DataContract]     public class OrderStatus     {         [DataMember(Name = "underlying_symbol")]         public string UnderlyingSymbol { get;
 set;
 }          [DataMember(Name = "price")]         public decimal Price { get;
 set;
 }          [DataMember(Name = "price-effect")]         public string PriceEffect { get;
 set;
 }          [DataMember(Name = "order-type")]         public string OrderType { get;
 set;
 }          [DataMember(Name = "status")]         public string Status { get;
 set;
 }          [DataMember(Name = "time-in-force")]         public string TimeInForce { get;
 set;
 }     } }
