﻿// Generated by Xamasoft JSON Class Generator
// http://www.xamasoft.com/json-class-generator

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace TWLib.Models
{
    public class Session
    {
        public class User2
        {

            [JsonProperty("email")]
            public string Email { get; set; }

            [JsonProperty("username")]
            public string Username { get; set; }

            [JsonProperty("external-id")]
            public string ExternalId { get; set; }
        }

        public class Data2
        {

            [JsonProperty("user")]
            public User2 User { get; set; }

            [JsonProperty("session-token")]
            public string SessionToken { get; set; }
        }


        [JsonProperty("data")]
        public Data2 Data { get; set; }

        [JsonProperty("context")]
        public string Context { get; set; }

    }
}
