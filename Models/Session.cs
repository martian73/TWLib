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

namespace TWLib.Models
{
    public class Session
    {
        public class User2
        {
            [JsonProperty("email")]
            public string Email
            {
                get;
                set;
            }
            [JsonProperty("username")]
            public string Username
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
        }
        public class Data2
        {
            [JsonProperty("user")]
            public User2 User
            {
                get;
                set;
            }
            [JsonProperty("session-token")]
            public string SessionToken
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
