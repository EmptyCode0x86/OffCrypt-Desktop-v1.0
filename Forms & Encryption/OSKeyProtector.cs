using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace OffCrypt
{
    /// <summary>
    /// Passwordless-suojaus: yksityisavain DPAPI:lla (CurrentUser) – ei salasanaa käyttäjälle.
    /// Tiedosto on käyttökelpoinen vain samalla Windows-tilillä.
    /// </summary>
    public static class OSKeyProtector
    {
        private const string Purpose = "OffCrypt-PrivateKey-DPAPI-v1";

        private sealed class KeyFile
        {
            public string ver { get; set; } = "1";
            public string alg { get; set; } = "DPAPI";
            public string scope { get; set; } = "CurrentUser";
            public string blob { get; set; } = "";     // base64 ProtectedData
        }

        public static string DefaultPath =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                         "OffCrypt", "keys", "rsa_priv.dpapi.json");

        public static void SavePrivateKey(RSA rsa, string? path = null)
        {
            path ??= DefaultPath;
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);

            byte[] pkcs8 = rsa.ExportPkcs8PrivateKey();
            try
            {
                byte[] entropy = Encoding.UTF8.GetBytes(Purpose);
                byte[] protectedBlob = ProtectedData.Protect(pkcs8, entropy, DataProtectionScope.CurrentUser);

                var kf = new KeyFile { blob = Convert.ToBase64String(protectedBlob) };
                File.WriteAllText(path, JsonSerializer.Serialize(kf));
            }
            finally
            {
                CryptographicOperations.ZeroMemory(pkcs8);
            }
        }

        public static RSA LoadPrivateKey(string? path = null)
        {
            path ??= DefaultPath;
            var kf = JsonSerializer.Deserialize<KeyFile>(File.ReadAllText(path))
                     ?? throw new InvalidDataException("Invalid key file");

            byte[] blob = Convert.FromBase64String(kf.blob);
            byte[] pkcs8 = ProtectedData.Unprotect(blob, Encoding.UTF8.GetBytes(Purpose),
                                                   DataProtectionScope.CurrentUser);

            try
            {
                var rsa = RSA.Create();
                rsa.ImportPkcs8PrivateKey(pkcs8, out _);
                return rsa;
            }
            finally
            {
                CryptographicOperations.ZeroMemory(pkcs8);
            }
        }
    }
}
