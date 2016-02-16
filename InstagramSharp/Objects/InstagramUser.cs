using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramSharp.Objects
{
    class InstagramUser
    {
        public string username { get; set; }
        public long pk { get; set; }
        public string profile_pic_url { get; set; }
        public bool is_private { get; set; }
        public string full_name { get; set; }
        public bool has_anonymous_profile_picture { get; set; }
        public string fbuid { get; set; }

        public InstagramUser(JToken JUser)
        {
            username = JUser["username"].ToString();
            pk = long.Parse(JUser["pk"].ToString());
            profile_pic_url = JUser["profile_pic_url"].ToString();
            is_private = bool.Parse(JUser["is_private"].ToString());
            full_name = JUser["full_name"].ToString();
            if(JUser["has_anonymous_profile_picture"] != null)
            {
                has_anonymous_profile_picture = bool.Parse(JUser["has_anonymous_profile_picture"].ToString());
            }
            if(JUser["fbuid"] != null)
            {
                fbuid = JUser["fbuid"].ToString();
            }
        }
    }
}
