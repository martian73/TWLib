﻿// Generated by Xamasoft JSON Class Generator
// http://www.xamasoft.com/json-class-generator

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TWLib.Models
{
    public class Positions
    {
        public class Data2
        {

            [JsonProperty("items")]
            public IList<object> Items { get; set; }
        }

        [JsonProperty("data")]
        public Data2 Data { get; set; }

        [JsonProperty("api-version")]
        public string ApiVersion { get; set; }

        [JsonProperty("context")]
        public string Context { get; set; }

    }
}