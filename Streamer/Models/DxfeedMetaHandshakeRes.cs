﻿// Generated by Xamasoft JSON Class Generator
// http://www.xamasoft.com/json-class-generator

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace TWLib.Streamer.Models
{
    public class DxFeedMetaHandshakeRes : DxfeedResponse
    {
        public class Advice2
        {

            [JsonProperty("interval")]
            public int Interval { get; set; }

            [JsonProperty("timeout")]
            public int Timeout { get; set; }

            [JsonProperty("reconnect")]
            public string Reconnect { get; set; }
        }


        [JsonProperty("minimumVersion")]
        public string MinimumVersion { get; set; }

        [JsonProperty("clientId")]
        public string ClientId { get; set; }

        [JsonProperty("supportedConnectionTypes")]
        public IList<string> SupportedConnectionTypes { get; set; }

        [JsonProperty("advice")]
        public Advice2 Advice { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("successful")]
        public bool Successful { get; set; }

        public override string Serialize()
        {
            DxFeedMetaHandshakeRes[] arr = new DxFeedMetaHandshakeRes[] { this };
            return JsonConvert.SerializeObject(arr);
        }

        public override TWResponse Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<DxFeedMetaHandshakeRes>(json);
        }
    }
}
