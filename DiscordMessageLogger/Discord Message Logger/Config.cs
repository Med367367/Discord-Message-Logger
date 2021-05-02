using DSharpPlus;
using System;
using System.IO;
using Discord_Message_Logger.Properties;
using System.Text;
using System.Management;

namespace Discord_Message_Logger.Startup
{
    public class Config
    {
        public static string SaveRoot = "./Saves";
        public static void ConfigChecks()
        {
            if (Settings.Default.Encrypt && Settings.Default.Token != "null") // If the token encryption is enabled and the token is not on default (null)
            {
                string Encrypted = Startup.token;
                Startup.token = Crypter.Decrypt(Encrypted, Config.GetUniqueMachineId());
            }

            if (Startup.token.Length < 59)// Verify the token Length
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("The token '"+ Startup.token + "' seem invalid, please verify your token");
                Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }

        public static void ConfigSaveSetup()
        {
            // DEFAULT
            if (!Directory.Exists(SaveRoot))
            {
                Directory.CreateDirectory(SaveRoot);
            }
            if (!Directory.Exists(SaveRoot + "/" + TokenType.Bot))
            {
                Directory.CreateDirectory(SaveRoot + "/" + TokenType.Bot);
            }
            if (!Directory.Exists(SaveRoot + "/" + TokenType.User))
            {
                Directory.CreateDirectory(SaveRoot + "/" + TokenType.User);
            }
            // CUSTOM FILE MATCH WITH CLIENT INFORMATION
            string ClientFullName = Startup.ClientName + "#" + Startup.ClientTag;
            if (!Directory.Exists(SaveRoot + "/" + Startup.TokenTyp + "/" + ClientFullName))
            {
                Directory.CreateDirectory(SaveRoot + "/" + Startup.TokenTyp + "/" + ClientFullName);
            }

        }

        static public string GetUniqueMachineId() 
        {
            StringBuilder builder = new StringBuilder();
            String query = "SELECT * FROM Win32_BIOS";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            foreach (ManagementObject item in searcher.Get())
            {
                Object obj = item["SerialNumber"];
                builder.Append(Convert.ToString(obj));
            }
            return builder.ToString();
        }
    }
}
