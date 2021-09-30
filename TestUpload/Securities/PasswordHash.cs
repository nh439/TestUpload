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
            using(var sha512 = SHA512.Create())
            {
                byte[] sourceBytes = Encoding.UTF8.GetBytes(p);
                byte[] hashBytes = sha512.ComputeHash(sourceBytes);
                hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty);
            }
            return hash;
        }
    }
}
