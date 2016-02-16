using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InstagramSharp.Objects
{
    class InstagramResponse
    {
        public WebHeaderCollection header { get; set; }
        [JsonProperty("status")]
        public string status { get; set; }
        [JsonProperty("message")]
        public string message { get; set; }
        public JObject rawJson { get; set; }

        public InstagramResponse()
        {
            header = new WebHeaderCollection();
            rawJson = new JObject();
        }
    }
}
