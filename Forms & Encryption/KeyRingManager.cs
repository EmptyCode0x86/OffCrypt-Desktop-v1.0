using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace OffCrypt
{
    public enum PassphraseMode { UserProvided, Random }

    public sealed class KeyMetadata
    {
        public string FingerprintHex { get; init; } = "";
        public string KeyId { get; init; } = "";      // esim. viimeiset 16 hex
        public string RandomId { get; init; } = "";   // 8 hex-merkkiä tunnistukseen
        public string DisplayName { get; init; } = ""; // Valinnainen nimi
        public DateTime CreatedUtc { get; init; }
    }

    public static class KeyringManager
    {
        public static string KeyringDir =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "OffCrypt", "keyring");

        private static string SecPath(string fpHex) => Path.Combine(KeyringDir, fpHex + ".ocsec");
        private static string PubPath(string fpHex) => Path.Combine(KeyringDir, fpHex + ".ocpub");

        public static KeyMetadata CreateIdentity(string? displayName, PassphraseMode mode,
                                                 out char[] shownPassphrase) // palautetaan UI:lle näytettäväksi jos Random
        {
            Directory.CreateDirectory(KeyringDir);
            shownPassphrase = Array.Empty<char>();

            using var rsa = RSA.Create(4096);

            // 1) Lasketaan fingerprint julkisesta avaimesta
            byte[] pubDer = rsa.ExportSubjectPublicKeyInfo();
            byte[] fp = SHA256.HashData(pubDer);
            string fpHex = Convert.ToHexString(fp);                // koko fingerprint
            string keyId = fpHex[^16..];                           // viimeiset 8 tavua = 16 hex

            // 2) Generoidaan satunnainen 8 hex-merkin ID tunnistukseen  
            string randomId = GenerateRandomId();
            
            // 3) Käyttäjän tunnistetieto (valinnainen nimi + randomId)
            string userId = string.IsNullOrWhiteSpace(displayName) ? randomId : $"{displayName} [{randomId}]";

            // 4) Passphrase (käytetään aina Random-moodia)
            if (mode != PassphraseMode.Random)
            {
                throw new NotSupportedException("Only Random passphrase mode is currently supported");
            }
            
            shownPassphrase = PasswordUtil.GenerateRandomPassphrase(24);
            string pass = new string(shownPassphrase);

            try
            {
                // 5) SALAINEN AVAIN (Argon2id + AES-GCM JSON → ASCII-armor)
                string json = KeyProtector.EncryptPkcs8ToJson(rsa.ExportPkcs8PrivateKey(), pass);
                string armoredSec = ArmorUtil.Encode("ENCRYPTED PRIVATE KEY", Encoding.UTF8.GetBytes(json));
                File.WriteAllText(SecPath(fpHex), armoredSec, Encoding.UTF8);

                // 6) JULKINEN AVAIN → PEM-muoto (sisältää Random ID kommentissa)
                string pubPem = ExportPublicKeyPem(rsa, displayName, randomId);
                File.WriteAllText(PubPath(fpHex), pubPem, Encoding.UTF8);

                // 7) SALATTU METADATA → DPAPI-suojattu
                var metadata = new KeyMetadata
                {
                    FingerprintHex = fpHex,
                    KeyId = keyId,
                    RandomId = randomId,
                    DisplayName = displayName ?? "",
                    CreatedUtc = DateTime.UtcNow
                };

                // Tallenna salattu metadata
                var encryptedMetadata = IdentityMetadata.FromKeyMetadata(metadata);
                encryptedMetadata.SaveEncrypted();

                return metadata;
            }
            finally
            {
                // Älä tyhjennä shownPassphrasea täällä, koska se palautetaan UI:lle näytettäväksi.
                // Kutsujan vastuulla on tyhjentää shownPassphrase turvallisesti näytön jälkeen.
            }
        }

        public static (RSA rsa, KeyMetadata meta) LoadSecretForUse(string fingerprintHex, string passphrase)
        {
            string armored = File.ReadAllText(SecPath(fingerprintHex), Encoding.UTF8);
            byte[] jsonBytes = ArmorUtil.Decode(armored, "ENCRYPTED PRIVATE KEY");
            string json = Encoding.UTF8.GetString(jsonBytes);

            // Purku (Argon2id + AES-GCM)
            byte[] pkcs8 = KeyProtector.DecryptJsonToPkcs8(json, passphrase);
            try
            {
                var rsa = RSA.Create();
                rsa.ImportPkcs8PrivateKey(pkcs8, out _);

                // Meta voidaan lukea publicista (käyttöön esim. fingerprint/keyId)
                string pubPem = File.ReadAllText(PubPath(fingerprintHex), Encoding.UTF8);
                byte[] pubDer = rsa.ExportSubjectPublicKeyInfo();
                string fpHex = Convert.ToHexString(SHA256.HashData(pubDer));
                string keyId = fpHex[^16..];

                // Parse userId to extract randomId and displayName (if available)
                string userId = ExtractUserIdFromPem(pubPem);
                var (randomId, displayName) = ParseUserId(userId);

                return (rsa, new KeyMetadata { 
                    FingerprintHex = fpHex, 
                    KeyId = keyId,
                    RandomId = randomId,
                    DisplayName = displayName 
                });
            }
            finally
            {
                CryptographicOperations.ZeroMemory(pkcs8);
            }
        }

        // Hae vastaanottajan julkinen avain RandomId:llä:
        public static string? FindPublicArmoredByRandomId(string randomId)
        {
            // Skannaa kaikki .ocpub ja etsi randomId
            foreach (var file in Directory.EnumerateFiles(KeyringDir, "*.ocpub"))
            {
                try
                {
                    var pubPem = File.ReadAllText(file, Encoding.UTF8);
                    string userId = ExtractUserIdFromPem(pubPem);
                    var (foundRandomId, _) = ParseUserId(userId);
                    
                    if (foundRandomId.Equals(randomId, StringComparison.OrdinalIgnoreCase))
                        return pubPem;
                }
                catch
                {
                    // Skip corrupted files
                    continue;
                }
            }
            return null;
        }


        // Julkisen avaimen PEM-vienti Random ID:n kanssa
        private static string ExportPublicKeyPem(RSA rsa, string? displayName = null, string? randomId = null)
        {
            var sb = new StringBuilder();
            sb.AppendLine("-----BEGIN PUBLIC KEY-----");
            
            // Lisää Comment-rivi Random ID:n kanssa
            if (!string.IsNullOrEmpty(randomId))
            {
                string comment = string.IsNullOrEmpty(displayName) 
                    ? $"Comment: [{randomId}]"
                    : $"Comment: {displayName} [{randomId}]";
                sb.AppendLine(comment);
            }
            
            // Tee PEM "BEGIN PUBLIC KEY" SubjectPublicKeyInfo-tavuille
            var spki = rsa.ExportSubjectPublicKeyInfo();
            var b64 = Convert.ToBase64String(spki);
            for (int i = 0; i < b64.Length; i += 64)
                sb.AppendLine(b64.Substring(i, Math.Min(64, b64.Length - i)));
            sb.AppendLine("-----END PUBLIC KEY-----");
            
            return sb.ToString();
        }

        // Generoi 8 hex-merkin satunnainen ID tunnistukseen
        private static string GenerateRandomId()
        {
            byte[] randomBytes = new byte[4]; // 4 tavua = 8 hex-merkkiä
            RandomNumberGenerator.Fill(randomBytes);
            return Convert.ToHexString(randomBytes);
        }

        // Parsii userId-stringin ja palauttaa (randomId, displayName)
        private static (string randomId, string displayName) ParseUserId(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return ("", "");

            // Jos muoto on "DisplayName [A7F3B8C2]"
            var match = System.Text.RegularExpressions.Regex.Match(userId, @"^(.+?)\s*\[([A-F0-9]{8})\]$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            if (match.Success)
                return (match.Groups[2].Value, match.Groups[1].Value.Trim());

            // Jos vain randomId (8 hex-merkkiä)
            if (System.Text.RegularExpressions.Regex.IsMatch(userId, @"^[A-F0-9]{8}$", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                return (userId, "");

            // Muuten käytä koko stringiä displayName-arvona
            return ("", userId);
        }

        // Poimi userId PEM-tiedostosta (yksinkertainen toteutus)
        private static string ExtractUserIdFromPem(string pemContent)
        {
            // Tämä on yksinkertainen toteutus - oikeassa PGP:ssä userId olisi omassa blokissaan
            // Toistaiseksi käytä tiedostonimeä fingerprint-pohjaisena tunnisteena
            try
            {
                // Etsi kommenttirivi jossa saattaa olla userId-tietoja
                var lines = pemContent.Split('\n');
                foreach (var line in lines)
                {
                    if (line.StartsWith("Comment:") && line.Contains("[") && line.Contains("]"))
                    {
                        // Etsi RandomID hakasulkeista
                        int start = line.LastIndexOf('[');
                        int end = line.LastIndexOf(']');
                        if (start >= 0 && end > start)
                        {
                            string randomId = line.Substring(start + 1, end - start - 1);
                            string displayName = "";
                            
                            // Jos kommentissa on nimi ennen hakasulkeita
                            string beforeBrackets = line.Substring(line.IndexOf(':') + 1, start - line.IndexOf(':') - 1).Trim();
                            if (beforeBrackets.Contains(" - "))
                            {
                                displayName = beforeBrackets.Split(" - ", StringSplitOptions.RemoveEmptyEntries).LastOrDefault()?.Trim() ?? "";
                            }
                            
                            return string.IsNullOrEmpty(displayName) ? randomId : $"{displayName} [{randomId}]";
                        }
                    }
                }
                
                return ""; // Ei löytynyt userId-tietoja
            }
            catch
            {
                return "";
            }
        }
    }
}
