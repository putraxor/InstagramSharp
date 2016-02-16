using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using InstagramSharp.Objects;

namespace InstagramSharp
{
    public class InstagramSharpClient
    {
        const string API_URL         = "https://i.instagram.com/api/v1/";
        const string USER_AGENT      = "Instagram 7.13.1 Android (23/6.0.1; 515dpi; 1440x2416; huawei/google; Nexus 6P; angler; angler; en_US)";
        const string IG_SIG_KEY      = "8b46309eb680f272cc770d214b7dbe5f0c5d26b6cb82b0b740257360b43618f0";
        const string SIG_KEY_VERSION = "4";

        private string username;
        private string password;
        private bool debug;

        private string uuid;
        private string device_id;
        private long username_id;
        private string token;
        private bool isLoggedIn = false;
        private string rank_token;
        private string IGDataPath;

        public InstagramSharpClient(string username, string password, bool debug = false, string IGDataPath = null)
        {

            this.username = username;
            this.password = password;
            this.debug = debug;
            
            this.uuid = this.generateUUID();
            this.device_id = this.generateDeviceId();
            
            if (IGDataPath != null)
                this.IGDataPath = IGDataPath;
            else
                this.IGDataPath = "/data/";

            if (!Directory.Exists(this.IGDataPath))
            {
                Directory.CreateDirectory(this.IGDataPath);
            }

            if ((File.Exists($"{this.IGDataPath}{this.username}-cookies.dat")) && (File.Exists($"{this.IGDataPath}{this.username}-userID.dat")) && (File.Exists($"{this.IGDataPath}{this.username}-token.dat")))
            {
                this.isLoggedIn = true;
                this.username_id = int.Parse(new StreamReader($"{this.IGDataPath}{this.username}-userId.dat").ReadToEnd().Trim());
                this.rank_token = $"{this.username_id}_{this.uuid}";
                this.token = new StreamReader($"{this.IGDataPath}{this.username}-token.dat").ReadToEnd().Trim();
            }

            if (this.debug)
            {
                Console.WriteLine($"Username {this.username} | Password {this.password} | Debug {this.debug}");
                Console.WriteLine($"UUID {this.uuid} | Device_ID {this.device_id}");
                Console.WriteLine($"IGDataPath {this.IGDataPath}");
                Console.WriteLine($"isLoggedIn {this.isLoggedIn} | username_id {this.username_id}");
                Console.WriteLine($"rank_token {this.rank_token} | token {this.token}");
            }
        }

        public string login()
        {
            if (!this.isLoggedIn)
            {
                InstagramHeaderResponse instaHeader = new InstagramHeaderResponse(this.request($"si/fetch_headers/?challenge_type=signup&guid={this.generateUUID()}"));
                string token = string.Empty;

                for (int i = 0; i < instaHeader.header.Count; i++)
                {
                    if (instaHeader.header.Keys[i] == "Set-Cookie")
                    {
                        token = instaHeader.header[i].Substring(10, 32);
                    }
                }

                IDictionary<string, string> data = new Dictionary<string, string>();
                data.Add("device_id", this.device_id);
                data.Add("guid", this.uuid);
                data.Add("username", this.username);
                data.Add("password", this.password);
                data.Add("csrftoken", token);
                data.Add("login_attempt_count", "0");

                InstagramLoginResponse instaLogin = new InstagramLoginResponse(this.request($"accounts/login/", this.generateSignature(JsonConvert.SerializeObject(data))));

                if(instaLogin.status == null)
                {
                    this.login();
                }
                
                this.isLoggedIn = true;
                this.username_id = instaLogin.logged_in_user.pk;

                this.rank_token = this.username_id + "_" + this.uuid;
                for (int i = 0; i < instaLogin.header.Count; i++)
                {
                    if (instaLogin.header.Keys[i] == "Set-Cookie")
                    {
                        token = instaLogin.header[i].Substring(10, 32);
                    }
                }
                this.token = token;

                return instaLogin.rawJson.ToString();
            }
            return "";
        }

        public bool logout()
        {
            if(!this.isLoggedIn)
            {
                throw new Exception("Not logged in");
            }

            InstagramResponse logout = this.request("accounts/logout/");
            

            return false;
        }

        public string getTimeline()
        {
            InstagramTimelineResponse response = new InstagramTimelineResponse(this.request($"feed/timeline/?rank_token={this.rank_token}&ranked_content=true"));

            Console.WriteLine(response.items[0].caption.text);

            return JsonConvert.SerializeObject(response);
        }

        public string generateSignature(string data)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();

            byte[] keyByte = encoding.GetBytes(IG_SIG_KEY);
            HMACSHA256 hmacsha256 = new HMACSHA256(keyByte);

            byte[] messageByte = encoding.GetBytes(data);
            byte[] hashmessage = hmacsha256.ComputeHash(messageByte);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashmessage.Length; i++)
            {
                sb.Append(hashmessage[i].ToString("x2"));
            }
            string hash = sb.ToString();

            return $"ig_sig_key_version={SIG_KEY_VERSION}&signed_body={hash}.{Uri.EscapeDataString(data)}";
        }

        public string generateDeviceId()
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            Random r = new Random();
            MD5 md5 = MD5.Create();
            byte[] inputBytes = encoding.GetBytes(r.Next(1000, 9999).ToString());
            byte[] hash = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }
            return $"android-{sb.ToString().Substring(0, 16)}";
        }

        public string generateUUID()
        {
            return Guid.NewGuid().ToString();
        }

        private InstagramResponse request(string endpoint, string post = null)
        {
            if (this.debug)
            {
                Console.WriteLine($"Request URL {API_URL}{endpoint}");
                Console.WriteLine($"{post}");
                Console.WriteLine($"");
            }

            try
            {                
                var httpRequest = (HttpWebRequest)WebRequest.Create($"{API_URL}{endpoint}");
                httpRequest.Method = "GET";
                httpRequest.UserAgent = USER_AGENT;
                httpRequest.CookieContainer = ReadCookiesFromDisk($"{this.IGDataPath}{this.username}-cookies.dat");

                if (post != null)
                {
                    httpRequest.Method = "POST";

                    byte[] postData = Encoding.ASCII.GetBytes(post);

                    using (var sw = new StreamWriter(httpRequest.GetRequestStream()))
                    {
                        sw.Write(post);
                        sw.Flush();
                        sw.Close();
                    }
                }

                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                WebHeaderCollection header = httpResponse.Headers;

                using (var sr = new StreamReader(httpResponse.GetResponseStream()))
                {
                    JObject body = JObject.Parse(sr.ReadToEnd());
                    
                    StreamWriter requestwriter = new StreamWriter("temp_webrequest.txt", true);
                    requestwriter.WriteLine($"URL: {API_URL}{endpoint}");
                    requestwriter.Flush();
                    requestwriter.WriteLine($"Body:");
                    requestwriter.WriteLine($"{body}");
                    requestwriter.WriteLine($"Header:");
                    for (int i = 0; i < header.Count; i++)
                    {
                        requestwriter.WriteLine($"{header.Keys[i]}: {header[i]}");
                    }
                    requestwriter.WriteLine($"");
                    requestwriter.Flush();
                    requestwriter.Close();

                    InstagramResponse response = new InstagramResponse()
                    {
                        header = header,
                        status = body["status"].ToString(),
                        rawJson = body,
                    };

                    if (body["message"] != null)
                    {
                        response.message = body["message"].ToString();
                    }

                    WriteCookiesToDisk($"{this.IGDataPath}{this.username}-cookies.dat", httpRequest.CookieContainer);

                    return response;
                }
            }
            catch (WebException ex) {
                StreamWriter sw = new StreamWriter("temp_webrequest_error.txt", true);

                sw.WriteLine($"{ex.Message}:");
                sw.WriteLine($"{ex.StackTrace}");

                using (Stream data = ex.Response.GetResponseStream())
                {
                    using (var reader = new StreamReader(data))
                    {
                        sw.WriteLine($"{reader.ReadToEnd()}");
                    }
                }
                sw.Flush();
                sw.Close();

                InstagramResponse response = new InstagramResponse() {
                    header = new WebHeaderCollection(),
                    status = "fail",
                    message = "Can't grab error message",
                    rawJson = new JObject(),
                };

                return response;
            }
        }

        public static void WriteCookiesToDisk(string file, CookieContainer cookieJar)
        {
            using (Stream stream = File.Create(file))
            {
                try
                {
                    //Console.Out.Write("Writing cookies to disk... ");
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, cookieJar);
                    //Console.Out.WriteLine("Done.");
                }
                catch (Exception e)
                {
                    //Console.Out.WriteLine("Problem writing cookies to disk: " + e.GetType());
                }
            }
        }

        public static CookieContainer ReadCookiesFromDisk(string file)
        {

            try
            {
                using (Stream stream = File.Open(file, FileMode.Open))
                {
                    //Console.Out.Write("Reading cookies from disk... ");
                    BinaryFormatter formatter = new BinaryFormatter();
                    //Console.Out.WriteLine("Done.");
                    return (CookieContainer)formatter.Deserialize(stream);
                }
            }
            catch (Exception e)
            {
                //Console.Out.WriteLine("Problem reading cookies from disk: " + e.GetType());
                return new CookieContainer();
            }
        }
    }
}
