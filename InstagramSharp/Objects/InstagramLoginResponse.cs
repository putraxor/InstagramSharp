using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramSharp.Objects
{
    class InstagramLoginResponse : InstagramResponse
    {
        public InstagramUser logged_in_user { get; internal set; }

        public InstagramLoginResponse(InstagramResponse response)
        {
            if(response.rawJson["logged_in_user"] != null)
            {
                logged_in_user = new InstagramUser(response.rawJson["logged_in_user"]);
            }
        }
    }
}
