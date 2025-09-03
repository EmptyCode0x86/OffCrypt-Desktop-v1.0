using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace OffCrypt
{
    public enum TrustLevel
    {
        Unknown = 0,      // Ensimmäinen kerta nähdään
        Untrusted = 1,    // Merkitty ei-luotettavaksi
        Known = 2,        // Käytetty aikaisemmin, automaattisesti tallennettu
        Verified = 3,     // Käyttäjä vahvistanut fingerprint out-of-band
        FullTrust = 4     // Täysi luottamus (esim. oma kontakti)
    }

    /// <summary>
    /// Kontaktin luottamustiedot - seuraa IdentityMetadata.cs mallia
    /// DPAPI-salattu tallennussysteemi fingerprint-pohjaisella entropialla
    /// </summary>
    public sealed class ContactTrustMetadata : IDisposable
    {
        public string ContactFingerprint { get; init; } = "";
        public string RandomId { get; init; } = "";
        public string DisplayName { get; init; } = "";
        public TrustLevel TrustLevel { get; set; } = TrustLevel.Unknown;
        public DateTime FirstSeen { get; set; } = DateTime.UtcNow;
        public DateTime LastSeen { get; set; } = DateTime.UtcNow;
        public DateTime LastVerified { get; set; } = DateTime.MinValue;
        public string Notes { get; set; } = "";
        public int MessageCount { get; set; } = 0; // Montako viestiä vaihdettu

        private bool _disposed = false;

        public static string ContactsDirectory =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "OffCrypt", "contacts");

        private static string TrustFilePath(string fingerprint) =>
            Path.Combine(ContactsDirectory, fingerprint + ".octrust");

        /// <summary>
        /// Lataa kontaktin trust-metadata levyltä (DPAPI-salattu)
        /// Käyttää samaa DPAPI-mallia kuin IdentityMetadata.cs
        /// </summary>
        public static ContactTrustMetadata? LoadFromDisk(string fingerprint)
        {
            try
            {
                string filePath = TrustFilePath(fingerprint);
                if (!File.Exists(filePath))
                    return null;

                // Lue DPAPI-salattu data
                byte[] encryptedData = File.ReadAllBytes(filePath);
                
                // Käytä fingerprint-pohjaista entropiaa (sama kuin IdentityMetadata.cs)
                byte[] entropyBytes = SHA256.HashData(Encoding.UTF8.GetBytes(fingerprint));
                
                // DPAPI-purku
                byte[] decryptedData = ProtectedData.Unprotect(encryptedData, entropyBytes, DataProtectionScope.CurrentUser);
                
                // JSON-deserialisointi
                string json = Encoding.UTF8.GetString(decryptedData);
                var metadata = JsonSerializer.Deserialize<ContactTrustMetadata>(json);
                
                return metadata;
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError("ContactTrustMetadata", $"Failed to load trust metadata for {fingerprint}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Tallenna trust-metadata levylle (DPAPI-salattu)
        /// </summary>
        public void SaveToDisk()
        {
            try
            {
                Directory.CreateDirectory(ContactsDirectory);
                
                // JSON-serialisointi
                string json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
                byte[] jsonData = Encoding.UTF8.GetBytes(json);
                
                // Fingerprint-pohjainen entropia
                byte[] entropyBytes = SHA256.HashData(Encoding.UTF8.GetBytes(ContactFingerprint));
                
                // DPAPI-salaus
                byte[] encryptedData = ProtectedData.Protect(jsonData, entropyBytes, DataProtectionScope.CurrentUser);
                
                // Tallenna tiedostoon
                string filePath = TrustFilePath(ContactFingerprint);
                File.WriteAllBytes(filePath, encryptedData);
                
                ApplicationLogger.LogInfo("ContactTrustMetadata", $"Trust metadata saved for {RandomId} ({DisplayName})");
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError("ContactTrustMetadata", $"Failed to save trust metadata: {ex.Message}");
                throw;
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                // Siivoa herkkä data muistista
                Notes = "";
                _disposed = true;
            }
        }
    }

    /// <summary>
    /// Kontaktien luottamustietojen hallinta - seuraa IdentityManager.cs mallia
    /// Integroituu olemassa olevaan KeyRingManager-infrastruktuuriin
    /// </summary>
    public sealed class ContactManager : IDisposable
    {
        private readonly Dictionary<string, ContactTrustMetadata> _contactCache = new();
        private readonly object _cacheLock = new object();
        private bool _disposed = false;

        /// <summary>
        /// Hae kontaktin trust-level fingerprint-perusteisesti
        /// </summary>
        public TrustLevel GetTrustLevel(string fingerprint)
        {
            if (string.IsNullOrWhiteSpace(fingerprint))
                return TrustLevel.Unknown;

            // Tarkista cache (thread-safe)
            lock (_cacheLock)
            {
                if (_contactCache.TryGetValue(fingerprint, out var cachedContact))
                {
                    // Päivitä LastSeen
                    cachedContact.LastSeen = DateTime.UtcNow;
                    cachedContact.SaveToDisk();
                    return cachedContact.TrustLevel;
                }
            }

            // Lataa levyltä
            var contact = ContactTrustMetadata.LoadFromDisk(fingerprint);
            if (contact != null)
            {
                lock (_cacheLock)
                {
                    _contactCache[fingerprint] = contact;
                }
                contact.LastSeen = DateTime.UtcNow;
                contact.SaveToDisk();
                return contact.TrustLevel;
            }

            return TrustLevel.Unknown;
        }

        /// <summary>
        /// Aseta kontaktin trust-level
        /// Integroituu KeyRingManager:in PEM-parsingiin
        /// </summary>
        public void SetTrustLevel(string fingerprint, string randomId, string displayName, TrustLevel level, string notes = "")
        {
            if (string.IsNullOrWhiteSpace(fingerprint))
                throw new ArgumentException("Fingerprint cannot be empty", nameof(fingerprint));

            var contact = ContactTrustMetadata.LoadFromDisk(fingerprint) ?? new ContactTrustMetadata
            {
                ContactFingerprint = fingerprint,
                RandomId = randomId,
                DisplayName = displayName,
                FirstSeen = DateTime.UtcNow
            };

            // Päivitä tiedot
            contact.TrustLevel = level;
            contact.LastSeen = DateTime.UtcNow;
            contact.Notes = notes;
            
            if (level >= TrustLevel.Verified)
                contact.LastVerified = DateTime.UtcNow;

            // Tallenna
            contact.SaveToDisk();
            
            // Päivitä cache (thread-safe)
            lock (_cacheLock)
            {
                _contactCache[fingerprint] = contact;
            }
            
            ApplicationLogger.LogInfo("ContactManager", 
                $"Trust level set to {level} for {randomId} ({displayName})");
        }

        /// <summary>
        /// Tuo kontakti PEM-tiedostosta - integroituu KeyRingManager:iin
        /// Parsii Comment-rivin RandomID ja DisplayName tiedot
        /// </summary>
        public void ImportFromPEM(string armoredPublicKey, TrustLevel initialTrust = TrustLevel.Known)
        {
            try
            {
                // Dekoodaa X25519 PUBLIC KEY armor (ei OFFCRYPT)
                byte[] spkiDer = ArmorUtil.Decode(armoredPublicKey, "PUBLIC KEY");
                // Laske fingerprint X25519 SPKI DER-datasta
                var fingerprintBytes = HashUtil.Fingerprint16(spkiDer);
                var fingerprint = Convert.ToHexString(fingerprintBytes);
                
                // Parsii identity-tiedot Comment-rivistä X25519 PEM:stä
                var (randomId, displayName) = ParseIdentityFromPEM(armoredPublicKey);
                
                // Tallenna trust-tiedot
                SetTrustLevel(fingerprint, randomId, displayName, initialTrust, 
                             $"Imported from PEM on {DateTime.Now:yyyy-MM-dd HH:mm}");
                
                ApplicationLogger.LogInfo("ContactManager", 
                    $"Contact imported: {randomId} ({displayName}) with trust level {initialTrust}");
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError("ContactManager", $"Failed to import contact from PEM: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Hae kaikki kontaktit - UI:ta varten
        /// </summary>
        public List<ContactTrustMetadata> GetAllContacts()
        {
            var contacts = new List<ContactTrustMetadata>();
            
            try
            {
                if (!Directory.Exists(ContactTrustMetadata.ContactsDirectory))
                    return contacts;

                foreach (var file in Directory.EnumerateFiles(ContactTrustMetadata.ContactsDirectory, "*.octrust"))
                {
                    try
                    {
                        var fingerprint = Path.GetFileNameWithoutExtension(file);
                        var contact = ContactTrustMetadata.LoadFromDisk(fingerprint);
                        if (contact != null)
                            contacts.Add(contact);
                    }
                    catch (Exception ex)
                    {
                        ApplicationLogger.LogWarning("ContactManager", $"Failed to load contact from {file}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError("ContactManager", $"Failed to enumerate contacts: {ex.Message}");
            }
            
            return contacts;
        }

        /// <summary>
        /// Merkitse viesti lähetetyksi/vastaanotetuksi - tilastointiin
        /// </summary>
        public void RecordMessage(string fingerprint)
        {
            var contact = ContactTrustMetadata.LoadFromDisk(fingerprint);
            if (contact != null)
            {
                contact.MessageCount++;
                contact.LastSeen = DateTime.UtcNow;
                contact.SaveToDisk();
                
                // Päivitä cache (thread-safe)
                lock (_cacheLock)
                {
                    _contactCache[fingerprint] = contact;
                }
            }
        }

        /// <summary>
        /// Poista kontakti - varovainen toiminto
        /// </summary>
        public void RemoveContact(string fingerprint)
        {
            try
            {
                string filePath = Path.Combine(ContactTrustMetadata.ContactsDirectory, fingerprint + ".octrust");
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    lock (_cacheLock)
                    {
                        _contactCache.Remove(fingerprint);
                    }
                    ApplicationLogger.LogWarning("ContactManager", $"Contact removed: {fingerprint}");
                }
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError("ContactManager", $"Failed to remove contact {fingerprint}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Laske fingerprint X25519 SPKI DER-datasta - sama logiikka kuin X25519Manager
        /// </summary>
        private static string CalculateFingerprintFromSPKI(byte[] spkiDer)
        {
            try
            {
                // Käytä samaa HashUtil.Fingerprint16 funktiota kuin X25519Manager
                byte[] fingerprintBytes = HashUtil.Fingerprint16(spkiDer);
                return Convert.ToHexString(fingerprintBytes);
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError("ContactManager", $"Failed to calculate fingerprint: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Parsii identity-tiedot Comment-rivistä X25519 PEM:stä
        /// Jos Comment-riviä ei löydy, käytä fingerprint short ID:nä
        /// </summary>
        private static (string randomId, string displayName) ParseIdentityFromPEM(string pemContent)
        {
            try
            {
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
                            if (!string.IsNullOrWhiteSpace(beforeBrackets))
                                displayName = beforeBrackets;
                            
                            return (randomId, displayName);
                        }
                    }
                }
                
                // Jos Comment-riviä ei löytynyt, käytä fingerprint lyhyenä ID:nä
                try
                {
                    byte[] spkiDer = ArmorUtil.Decode(pemContent, "PUBLIC KEY");
                    byte[] fingerprintBytes = HashUtil.Fingerprint16(spkiDer);
                    string shortId = Convert.ToHexString(fingerprintBytes)[..8];
                    return (shortId, "");
                }
                catch
                {
                    return ("", "");
                }
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError("ContactManager", $"Failed to parse identity from PEM: {ex.Message}");
                return ("", "");
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                // Siivoa cache (thread-safe)
                lock (_cacheLock)
                {
                    foreach (var contact in _contactCache.Values)
                        contact?.Dispose();
                    _contactCache.Clear();
                }
                
                _disposed = true;
            }
        }
    }
}