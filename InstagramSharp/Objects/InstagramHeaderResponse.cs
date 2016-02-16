using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramSharp.Objects
{
    class InstagramHeaderResponse : InstagramResponse
    {
        public int shift { get; set; }
        public string response_header { get; set; }
        public int edges { get; set; }
        public int iterations { get; set; }
        public int size { get; set; }

        public InstagramHeaderResponse(InstagramResponse response)
        {
            status = response.rawJson["status"].ToString();
            shift = int.Parse(response.rawJson["shift"].ToString());
            response_header = response.rawJson["header"].ToString();
            edges = int.Parse(response.rawJson["edges"].ToString());
            iterations = int.Parse(response.rawJson["iterations"].ToString());
            size = int.Parse(response.rawJson["size"].ToString());
        }
    }
}
