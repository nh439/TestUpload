using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace TestUpload.Securities
{
    public class PasswordHash
    {
        public string Create(string key,string password)
        {
            string p = key + password + password + key + key + password;
            string hash;
            for (int i= 0; i <= 10;i++)
            {
                p = EncodeTo64(p);
            }
            using(var sha512 = SHA512.Create())
            {
                byte[] sourceBytes = Encoding.UTF8.GetBytes(p);
                byte[] hashBytes = sha512.ComputeHash(sourceBytes);
                hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty);
            }
            return hash;
        }

        public string CreateEncrypted(string key, string password)
        {
            // string p = key + password + password + key + key + password;
            string p = password + key + key + password + password + key;
            string hash;
            for (int i = 0; i <= 10; i++)
            {
                p = EncodeTo64(p);
            }
            using (var md5 = MD5.Create())
            {
                byte[] sourceBytes = Encoding.UTF8.GetBytes(p);
                byte[] hashBytes = md5.ComputeHash(sourceBytes);
                hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty);
            }
            return hash;
        }

        public string DecodeFrom64(string encodedData)

        {

            byte[] encodedDataAsBytes

                = System.Convert.FromBase64String(encodedData);

            string returnValue =

               System.Text.ASCIIEncoding.ASCII.GetString(encodedDataAsBytes);

            return returnValue;

        }
         public string EncodeTo64(string toEncode)

        {

            byte[] toEncodeAsBytes

                  = System.Text.ASCIIEncoding.ASCII.GetBytes(toEncode);

            string returnValue

                  = System.Convert.ToBase64String(toEncodeAsBytes);

            return returnValue;

        }
    }
}
