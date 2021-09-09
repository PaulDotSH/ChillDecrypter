using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ChillDecrypter
{
    class Encryption
    {
        public static byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes, byte[] SaltBytes)
        {
            byte[] decryptedBytes = null;
            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {

                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, SaltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    }
                    decryptedBytes = ms.ToArray();
                }
            }

            return decryptedBytes;
        }
        public static void DecryptFile(string file, byte[] PasswordBytes, byte[] SaltBytes)
        {
            byte[] BytesToBeDecrypted = File.ReadAllBytes(file);
            
            byte[] BytesDecrypted = AES_Decrypt(BytesToBeDecrypted, PasswordBytes, SaltBytes);

            File.WriteAllBytes(file, BytesDecrypted);
            string result = file.Substring(0, file.Length - Path.GetExtension(file).Length);
            File.Move(file, result);
            Console.Title = $"Chill Decryptor | {++DecryptedFiles} files decrypted";
        }

        static ulong DecryptedFiles = 0;

        public static void DecryptDirectory(ref byte[] PwBytes, ref string location, ref byte[] SaltBytes)
        {
            string[] files = Directory.GetFiles(location);
            string[] childDirectories = Directory.GetDirectories(location);
            for (int i = 0; i < files.Length; i++)
            {
                string extension = Path.GetExtension(files[i]);
                if (extension == Settings.EncryptedFilesExtension)
                {
                    try
                    {
                        DecryptFile(files[i], PwBytes, SaltBytes);
                        Console.WriteLine($"Successfully decrypted {files[i]}");
                    }
                    catch { }
                    //Console.WriteLine("Decrypted file from path: ");
                }
            }
            for (int i = 0; i < childDirectories.Length; i++)
            {
                DecryptDirectory(ref PwBytes, ref childDirectories[i], ref SaltBytes);
            }

        }
    }
}
