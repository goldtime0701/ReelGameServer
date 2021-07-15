using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ReelServer
{
    public static class CEncrypt
    {
        private static DES _des = new DES("solimcom");

        public static string Encrypt(string strInput)
        {
            return _des.result(true, strInput);
        }

        public static string Decrypt(string strInput)
        {
            char[] charsToTrim = { '\0' };
            string str = _des.result(false, strInput).TrimEnd(charsToTrim);

            return str;
        }
    }

    public class DES
    {
        private byte[] Key { get; set; }

        public DES(string strKey)
        {
            Key = ASCIIEncoding.ASCII.GetBytes(strKey);
        }


        public string result(bool bType, string strInput)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider()
            {
                Key = Key,
                IV = Key
            };

            MemoryStream ms = new MemoryStream();

            var property = new
            {
                transform = bType ? des.CreateEncryptor() : des.CreateDecryptor(),
                data = bType ? Encoding.UTF8.GetBytes(strInput.ToCharArray()) : Convert.FromBase64String(strInput)
            };

            CryptoStream cryStream = new CryptoStream(ms, property.transform, CryptoStreamMode.Write);
            var data = property.data;

            cryStream.Write(data, 0, data.Length);
            cryStream.FlushFinalBlock();

            return bType ? Convert.ToBase64String(ms.ToArray()) : Encoding.UTF8.GetString(ms.GetBuffer());
        }
    }
}
