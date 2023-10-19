using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace PruebaLogin.Tools
{
    public class EncriptaMD5
    {
        public byte[] Encrypt(string password)
        {
            string hash = "loginprueba";
            byte[] data = UTF8Encoding.UTF8.GetBytes(password);

            MD5 mD5 = MD5.Create();
            TripleDES tripleDES = TripleDES.Create();

            tripleDES.Key = mD5.ComputeHash(UTF8Encoding.UTF8.GetBytes(hash));
            tripleDES.Mode = CipherMode.ECB;

            ICryptoTransform cryptoTransform = tripleDES.CreateEncryptor();
            byte[] result = cryptoTransform.TransformFinalBlock(data, 0 , data.Length);

            return result;
        }

        public string Decrypt(string password)
        {
            string hash = "loginprueba";
            byte[] data = Convert.FromBase64String(password);

            MD5 mD5 = MD5.Create();
            TripleDES tripleDES = TripleDES.Create();

            tripleDES.Key = mD5.ComputeHash(UTF8Encoding.UTF8.GetBytes(hash));
            tripleDES.Mode = CipherMode.ECB;

            ICryptoTransform cryptoTransform = tripleDES.CreateDecryptor();
            byte[] result = cryptoTransform.TransformFinalBlock(data, 0 , data.Length);

            return UTF8Encoding.UTF8.GetString(result);
        }
    }
}