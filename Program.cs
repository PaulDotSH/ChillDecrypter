using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ChillDecrypter
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(@"
   _____ _     _ _ _ _____                             _            
  / ____| |   (_) | |  __ \                           | |           
 | |    | |__  _| | | |  | | ___  ___ _ __ _   _ _ __ | |_ ___ _ __ 
 | |    | '_ \| | | | |  | |/ _ \/ __| '__| | | | '_ \| __/ _ \ '__|
 | |____| | | | | | | |__| |  __/ (__| |  | |_| | |_) | ||  __/ |   
  \_____|_| |_|_|_|_|_____/ \___|\___|_|   \__, | .__/ \__\___|_|   
                                            __/ | |                 
                                           |___/|_|                 ");
            Console.Title = "ChillDecryptor";

            //Remove \n that might be inserted by mistake
            Console.WriteLine("Please enter your key");
            string key = Console.ReadLine().Replace("\n","");
            Console.WriteLine("Please enter the salt");
            string salt = Console.ReadLine().Replace("\n", "");

            byte[] SaltBytes = Convert.FromBase64String(salt);
            byte[] PasswordBytes = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(key));

            foreach (DriveInfo d in DriveInfo.GetDrives())
            {
                if (d.IsReady == true)
                {
                    string DriveName = d.Name;
                    try { Encryption.DecryptDirectory(ref PasswordBytes, ref DriveName, ref SaltBytes); } catch { }    
                }
            }
        }


    }
}
