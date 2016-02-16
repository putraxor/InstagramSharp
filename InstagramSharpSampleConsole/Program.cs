using InstagramSharp;
using InstagramSharp.Objects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramSharpSampleConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            InstagramSharpClient client = new InstagramSharpClient(Console.ReadLine(), Console.ReadLine(), true);
            client.login();
            using (StreamWriter sw = new StreamWriter("timeline.txt", false))
            {
                sw.WriteLine(client.getTimeline());
                sw.Flush();
                sw.Close();
            }
            client.logout();
            do
            {
                Console.ReadLine();
            } while (true);
        }
    }
}
