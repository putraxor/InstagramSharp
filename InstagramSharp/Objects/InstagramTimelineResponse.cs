using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramSharp.Objects
{
    class InstagramTimelineResponse : InstagramResponse
    {
        public int num_results { get; set; }
        public bool is_direct_v2_enabled { get; set; }
        public bool auto_load_more_enabled { get; set; }
        public List<InstagramTimelineItem> items { get; set; }
        public bool more_available { get; set; }

        public InstagramTimelineResponse(InstagramResponse response)
        {
            num_results = int.Parse(response.rawJson["num_results"].ToString());
            is_direct_v2_enabled = bool.Parse(response.rawJson["is_direct_v2_enabled"].ToString());
            auto_load_more_enabled = bool.Parse(response.rawJson["auto_load_more_enabled"].ToString());
            items = new List<InstagramTimelineItem>();
            foreach (JToken JItem in response.rawJson["items"])
            {
                InstagramTimelineItem item = new InstagramTimelineItem(JItem);
                items.Add(item);
            }
            more_available = bool.Parse(response.rawJson["more_available"].ToString());
        }
    }
}
