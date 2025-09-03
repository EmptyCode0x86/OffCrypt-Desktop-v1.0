using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace OffCrypt
{
    /// <summary>
    /// Encrypted metadata for identity information
    /// Protects sensitive identity data using Windows DPAPI
    /// </summary>
    public sealed class IdentityMetadata
    {
        public string FingerprintHex { get; init; } = "";
        public string KeyId { get; init; } = "";
        public string RandomId { get; init; } = "";
        public string DisplayName { get; init; } = "";
        public DateTime CreatedUtc { get; init; }
        public DateTime LastUsed { get; set; }
        
        /// <summary>
        /// Creates path for encrypted metadata file
        /// </summary>
        private static string MetadataPath(string fingerprintHex) => 
            Path.Combine(KeyringManager.KeyringDir, fingerprintHex + ".ocmeta");

        /// <summary>
        /// Saves identity metadata encrypted with DPAPI
        /// </summary>
        public void SaveEncrypted()
        {
            try
            {
                Directory.CreateDirectory(KeyringManager.KeyringDir);
                
                // Serialize to JSON
                var jsonOptions = new JsonSerializerOptions 
                { 
                    WriteIndented = false 
                };
                string json = JsonSerializer.Serialize(this, jsonOptions);
                byte[] jsonBytes = Encoding.UTF8.GetBytes(json);

                // Create entropy from fingerprint (deterministic but unique per identity)
                byte[] entropy = SHA256.HashData(Encoding.UTF8.GetBytes(FingerprintHex));

                // Encrypt with DPAPI (CurrentUser scope)
                byte[] encryptedData = ProtectedData.Protect(
                    jsonBytes,
                    entropy,
                    DataProtectionScope.CurrentUser
                );

                // Save to file
                File.WriteAllBytes(MetadataPath(FingerprintHex), encryptedData);
                
                System.Diagnostics.Debug.WriteLine($"Saved encrypted metadata for identity: {RandomId}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to save encrypted metadata: {ex.Message}");
                throw new InvalidOperationException($"Failed to save identity metadata: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Loads identity metadata decrypted from DPAPI
        /// </summary>
        public static IdentityMetadata? LoadEncrypted(string fingerprintHex)
        {
            try
            {
                string metadataFile = MetadataPath(fingerprintHex);
                if (!File.Exists(metadataFile))
                {
                    System.Diagnostics.Debug.WriteLine($"No encrypted metadata found for fingerprint: {fingerprintHex[..8]}...");
                    return null;
                }

                // Read encrypted data
                byte[] encryptedData = File.ReadAllBytes(metadataFile);
                
                // Create entropy from fingerprint
                byte[] entropy = SHA256.HashData(Encoding.UTF8.GetBytes(fingerprintHex));

                // Decrypt with DPAPI
                byte[] decryptedData = ProtectedData.Unprotect(
                    encryptedData,
                    entropy,
                    DataProtectionScope.CurrentUser
                );

                // Deserialize from JSON
                string json = Encoding.UTF8.GetString(decryptedData);
                var metadata = JsonSerializer.Deserialize<IdentityMetadata>(json);
                
                System.Diagnostics.Debug.WriteLine($"Loaded encrypted metadata for identity: {metadata?.RandomId}");
                return metadata;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load encrypted metadata: {ex.Message}");
                // Return null instead of throwing - allows graceful degradation
                return null;
            }
        }

        /// <summary>
        /// Updates last used timestamp and saves
        /// </summary>
        public void UpdateLastUsed()
        {
            LastUsed = DateTime.UtcNow;
            SaveEncrypted();
        }

        /// <summary>
        /// Deletes encrypted metadata file
        /// </summary>
        public void DeleteEncryptedFile()
        {
            try
            {
                string metadataFile = MetadataPath(FingerprintHex);
                if (File.Exists(metadataFile))
                {
                    File.Delete(metadataFile);
                    System.Diagnostics.Debug.WriteLine($"Deleted encrypted metadata for identity: {RandomId}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to delete encrypted metadata: {ex.Message}");
            }
        }

        /// <summary>
        /// Creates metadata from KeyMetadata (for backward compatibility)
        /// </summary>
        public static IdentityMetadata FromKeyMetadata(KeyMetadata keyMetadata)
        {
            return new IdentityMetadata
            {
                FingerprintHex = keyMetadata.FingerprintHex,
                KeyId = keyMetadata.KeyId,
                RandomId = keyMetadata.RandomId,
                DisplayName = keyMetadata.DisplayName,
                CreatedUtc = keyMetadata.CreatedUtc,
                LastUsed = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Converts to KeyMetadata (for backward compatibility)
        /// </summary>
        public KeyMetadata ToKeyMetadata()
        {
            return new KeyMetadata
            {
                FingerprintHex = FingerprintHex,
                KeyId = KeyId,
                RandomId = RandomId,
                DisplayName = DisplayName,
                CreatedUtc = CreatedUtc
            };
        }

        /// <summary>
        /// Checks if metadata file exists for given fingerprint
        /// </summary>
        public static bool ExistsForFingerprint(string fingerprintHex)
        {
            return File.Exists(MetadataPath(fingerprintHex));
        }
    }
}