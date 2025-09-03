using System;
using System.Security.Cryptography;

namespace OffCrypt
{
    public static class PasswordUtil
    {
       
        private const string Alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789!@#$%^*-_=+";


        public static char[] GenerateRandomPassphrase(int length = 24)
        {
            if (length <= 0) throw new ArgumentOutOfRangeException(nameof(length));
            var chars = new char[length];
            for (int i = 0; i < length; i++)
            {
                int idx = RandomNumberGenerator.GetInt32(Alphabet.Length); // tasajakautunut
                chars[i] = Alphabet[idx];
            }
            return chars;
        }

        public static byte[] GenerateRandomAesKey(int sizeBytes = 32)
        {
            var key = new byte[sizeBytes];
            RandomNumberGenerator.Fill(key);
            return key;
        }
    }
}
