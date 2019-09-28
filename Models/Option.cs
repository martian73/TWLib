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

using System;

namespace TWLib.Models
{
    public enum OptionType { PUT = 'P', CALL = 'C' }
    public class Option : Security
    {
        public string Ticker
        {
            get;
            set;
        }
        public DateTime Expiry
        {
            get;
            set;
        }
        public decimal Strike
        {
            get;
            set;
        }
        public OptionType OptionType
        {
            get;
            set;
        }
        public UnderlyingType UnderlyingType
        {
            get;
            set;
        }
    }
}
