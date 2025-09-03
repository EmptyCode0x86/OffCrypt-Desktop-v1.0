using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Security.Cryptography;
using Konscious.Security.Cryptography;

namespace OffCrypt
{
    /// <summary>
    /// Suojaa RSA PKCS#8 -yksityisavaimen Argon2id + AES-GCM -menetelmällä (ristialustainen).
    /// Tallennusformaatti: JSON (Base64-kentät suola/iv/tag/ct).
    /// </summary>
    public static class KeyProtector
    {
        // JSON-rakenne tiedostolle (versioitu)
        private sealed class KeyFile
        {
            public string ver { get; set; } = "1";
            public string rsa { get; set; } = "pkcs8";
            public string alg { get; set; } = "AES-256-GCM";
            public string kdf { get; set; } = "Argon2id";
            public int memKiB { get; set; }   // Argon2: Memory (KiB)
            public int iters { get; set; }    // Argon2: Iterations
            public int parallel { get; set; } // Argon2: Degree of parallelism
            public string salt { get; set; }  // b64
            public string iv { get; set; }    // b64
            public string tag { get; set; }   // b64
            public string ct { get; set; }    // b64
        }

        // Oletus Argon2id -parametrit (tasapaino: desktop)
        // Voit säätää: enemmän muistia = hitaampi hyökkääjälle.
        private const int DefaultMemKiB = 64 * 1024; // 64 MiB
        private const int DefaultIters = 3;
        private const int DefaultPar = 4;

        public static byte[] DeriveKeyArgon2id(string password, byte[] salt, int memKiB, int iters, int parallel)
        {
            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = salt,
                MemorySize = memKiB,            // KiB
                Iterations = iters,
                DegreeOfParallelism = parallel
            };
            try
            {
                return argon2.GetBytes(32);     // 256-bit AES key
            }
            finally
            {
                
            }
        }

        /// <summary>
        /// Salaa PKCS#8-yksityisavaimen (DER-bitteinä) salasanalla ja palauttaa JSON-stringin.
        /// Turvallinen versio joka käyttää char[] salasanaa.
        /// </summary>
        public static string EncryptPkcs8ToJson(byte[] privateKeyPkcs8Der, char[] password,
                                                int memKiB = DefaultMemKiB, int iters = DefaultIters, int parallel = DefaultPar)
        {
            if (password == null || password.Length == 0)
                throw new ArgumentException("Password required", nameof(password));

            // Muunna char[] väliaikaisesti string:ksi, käytä existing metodia, tyhjennä
            string passwordString = new string(password);
            try
            {
                return EncryptPkcs8ToJson(privateKeyPkcs8Der, passwordString, memKiB, iters, parallel);
            }
            finally
            {
                // Yritä tyhjentää string (ei täysin luotettava, mutta parempi kuin ei mitään)
                passwordString = string.Empty;
            }
        }

        /// <summary>
        /// Salaa PKCS#8-yksityisavaimen (DER-bitteinä) salasanalla ja palauttaa JSON-stringin.
        /// Legacy string-versio (käytä mieluummin char[] versiota).
        /// </summary>
        public static string EncryptPkcs8ToJson(byte[] privateKeyPkcs8Der, string password,
                                                int memKiB = DefaultMemKiB, int iters = DefaultIters, int parallel = DefaultPar)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password required", nameof(password));

            byte[] salt = new byte[16];
            RandomNumberGenerator.Fill(salt);

            byte[] key = DeriveKeyArgon2id(password, salt, memKiB, iters, parallel);

            byte[] iv = new byte[12];  // 96-bit nonce (GCM standard)
            RandomNumberGenerator.Fill(iv);

            byte[] ct = new byte[privateKeyPkcs8Der.Length];
            byte[] tag = new byte[16];  // 128-bit auth tag

            try
            {
                using var gcm = new AesGcm(key, 16);
                // Voit halutessasi lisätä AAD:ia (esim. header), ei pakollinen
                gcm.Encrypt(iv, privateKeyPkcs8Der, ct, tag);

                var kf = new KeyFile
                {
                    memKiB = memKiB,
                    iters = iters,
                    parallel = parallel,
                    salt = Convert.ToBase64String(salt),
                    iv = Convert.ToBase64String(iv),
                    tag = Convert.ToBase64String(tag),
                    ct = Convert.ToBase64String(ct)
                };

                var json = JsonSerializer.Serialize(kf, new JsonSerializerOptions { WriteIndented = false });
                return json;
            }
            finally
            {
                CryptographicOperations.ZeroMemory(key);
                // privateKeyPkcs8Der nollataan kutsujalla jos tarve
            }
        }

        /// <summary>
        /// Purkaa JSON-tiedoston yksityisavaimen (DER-bitteinä) salasanalla.
        /// Turvallinen versio joka käyttää char[] salasanaa.
        /// </summary>
        public static byte[] DecryptJsonToPkcs8(string json, char[] password)
        {
            if (password == null || password.Length == 0)
                throw new ArgumentException("Password required", nameof(password));

            // Muunna char[] väliaikaisesti string:ksi, käytä existing metodia, tyhjennä
            string passwordString = new string(password);
            try
            {
                return DecryptJsonToPkcs8(json, passwordString);
            }
            finally
            {
                // Yritä tyhjentää string (ei täysin luotettava, mutta parempi kuin ei mitään)
                passwordString = string.Empty;
            }
        }

        /// <summary>
        /// Purkaa JSON-tiedoston yksityisavaimen (DER-bitteinä) salasanalla.
        /// Legacy string-versio (käytä mieluummin char[] versiota).
        /// </summary>
        public static byte[] DecryptJsonToPkcs8(string json, string password)
        {
            var kf = JsonSerializer.Deserialize<KeyFile>(json) ?? throw new InvalidDataException("Invalid key file");

            if (kf.ver != "1" || kf.kdf != "Argon2id" || kf.alg != "AES-256-GCM" || kf.rsa != "pkcs8")
                throw new NotSupportedException("Unsupported key file format");

            byte[] salt = Convert.FromBase64String(kf.salt);
            byte[] iv = Convert.FromBase64String(kf.iv);
            byte[] tag = Convert.FromBase64String(kf.tag);
            byte[] ct = Convert.FromBase64String(kf.ct);

            byte[] key = DeriveKeyArgon2id(password, salt, kf.memKiB, kf.iters, kf.parallel);

            byte[] pkcs8 = new byte[ct.Length];
            try
            {
                using var gcm = new AesGcm(key, 16);
                gcm.Decrypt(iv, ct, tag, pkcs8);
                return pkcs8;
            }
            catch (CryptographicException)
            {
                throw new CryptographicException("Incorrect password or corrupted key file");
            }
            finally
            {
                CryptographicOperations.ZeroMemory(key);
            }
        }

        /// <summary>
        /// Apuri: talleta tiedostoon atomisesti.
        /// </summary>
        public static void SaveJsonAtomically(string path, string json)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            var tmp = path + ".tmp";
            File.WriteAllText(tmp, json, Encoding.UTF8);
            if (File.Exists(path)) File.Replace(tmp, path, null);
            else File.Move(tmp, path);
        }
    }
}
