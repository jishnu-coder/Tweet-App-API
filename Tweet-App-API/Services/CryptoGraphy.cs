using System;
using System.Security.Cryptography;
using System.Text;

namespace Tweet_App_API.Services
{
    public static class CryptoGraphy
    {
        //Create the hash value of the source
        public static string GetHash(string source)
        {
            string hashValue;

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(source));

                hashValue = Convert.ToBase64String(bytes);
            }

            return hashValue;
        }

        //Compare the hash values
        public static Boolean CompareHash(string newHashValue, string compareHash)
        {
            bool beEqual = false;

            if (newHashValue.Length == compareHash.Length)
            {
                int i = 0;
                while ((i < newHashValue.Length) && newHashValue[i] == compareHash[i])
                {
                    i += 1;
                }

                if (i == newHashValue.Length)
                {
                    beEqual = true;
                    return beEqual;
                }
            }

            return beEqual;

        }
    }
}
