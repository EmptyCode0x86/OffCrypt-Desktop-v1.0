using System;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace OffCrypt
{
    /// <summary>
    /// Contains recipient identity information parsed from public key
    /// Used for identity verification and message encryption
    /// </summary>
    public sealed record RecipientIdentity
    {
        public string PublicKeyPEM { get; init; } = "";
        public string RandomId { get; init; } = "";
        public string DisplayName { get; init; } = "";
        public string Fingerprint { get; init; } = "";
        public DateTime ImportedAt { get; init; }

        public RecipientIdentity()
        {
            ImportedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Creates RecipientIdentity from imported public key PEM
        /// Parses Comment field for Random ID and DisplayName
        /// </summary>
        public static RecipientIdentity FromPublicKeyPEM(string publicKeyPEM)
        {
            try
            {
                var identity = new RecipientIdentity
                {
                    PublicKeyPEM = publicKeyPEM.Trim(),
                    Fingerprint = CalculateFingerprint(publicKeyPEM)
                };

                // Parse Comment field for Random ID and DisplayName
                var (randomId, displayName) = ParseCommentField(publicKeyPEM);
                
                return identity with 
                { 
                    RandomId = randomId, 
                    DisplayName = displayName 
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error parsing recipient identity: {ex.Message}");
                
                // Return basic identity if parsing fails
                return new RecipientIdentity
                {
                    PublicKeyPEM = publicKeyPEM.Trim(),
                    RandomId = "",
                    DisplayName = "",
                    Fingerprint = CalculateFingerprint(publicKeyPEM)
                };
            }
        }

        /// <summary>
        /// Parses Comment field from public key PEM
        /// Expected formats: "Comment: [A1B2C3D4]" or "Comment: Alice [A1B2C3D4]"
        /// </summary>
        private static (string randomId, string displayName) ParseCommentField(string publicKeyPEM)
        {
            try
            {
                var lines = publicKeyPEM.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                
                foreach (var line in lines)
                {
                    var trimmedLine = line.Trim();
                    if (trimmedLine.StartsWith("Comment:", StringComparison.OrdinalIgnoreCase))
                    {
                        // Remove "Comment: " prefix
                        var commentContent = trimmedLine.Substring(8).Trim();
                        
                        // Match pattern: "Alice [A1B2C3D4]" or "[A1B2C3D4]"
                        var match = Regex.Match(commentContent, @"^(?:(.*?)\s*)?\[([A-F0-9]{8})\]$", RegexOptions.IgnoreCase);
                        
                        if (match.Success)
                        {
                            string displayName = match.Groups[1].Value.Trim();
                            string randomId = match.Groups[2].Value.ToUpper();
                            
                            return (randomId, displayName);
                        }
                    }
                }
                
                return ("", "");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error parsing comment field: {ex.Message}");
                return ("", "");
            }
        }

        /// <summary>
        /// Calculates fingerprint from public key PEM
        /// </summary>
        private static string CalculateFingerprint(string publicKeyPEM)
        {
            try
            {
                // Extract the public key content (remove PEM headers and whitespace)
                var lines = publicKeyPEM.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                var keyContent = "";
                bool inKeySection = false;
                
                foreach (var line in lines)
                {
                    var trimmedLine = line.Trim();
                    if (trimmedLine.StartsWith("-----BEGIN"))
                    {
                        inKeySection = true;
                        continue;
                    }
                    if (trimmedLine.StartsWith("-----END"))
                    {
                        break;
                    }
                    if (inKeySection && !trimmedLine.StartsWith("Comment:"))
                    {
                        keyContent += trimmedLine;
                    }
                }
                
                // Decode base64 and calculate SHA256
                byte[] keyBytes = Convert.FromBase64String(keyContent);
                byte[] hash = SHA256.HashData(keyBytes);
                return Convert.ToHexString(hash);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error calculating fingerprint: {ex.Message}");
                return "";
            }
        }

        /// <summary>
        /// Gets display text for UI
        /// </summary>
        public string GetDisplayText()
        {
            if (!string.IsNullOrEmpty(DisplayName) && !string.IsNullOrEmpty(RandomId))
            {
                return $"{DisplayName} [{RandomId}]";
            }
            else if (!string.IsNullOrEmpty(RandomId))
            {
                return $"[{RandomId}]";
            }
            else if (!string.IsNullOrEmpty(DisplayName))
            {
                return DisplayName;
            }
            else
            {
                string shortFingerprint = string.IsNullOrEmpty(Fingerprint) || Fingerprint.Length < 8 
                    ? "UNKNOWN" 
                    : Fingerprint[..8];
                return $"Unknown [{shortFingerprint}...]";
            }
        }

        /// <summary>
        /// Checks if this identity has valid Random ID
        /// </summary>
        public bool HasValidRandomId()
        {
            return !string.IsNullOrEmpty(RandomId) && 
                   RandomId.Length == 8 && 
                   Regex.IsMatch(RandomId, @"^[A-F0-9]{8}$");
        }

        /// <summary>
        /// Returns short summary for logging
        /// </summary>
        public override string ToString()
        {
            string shortFingerprint = string.IsNullOrEmpty(Fingerprint) || Fingerprint.Length < 8 
                ? "UNKNOWN" 
                : Fingerprint[..8];
            return $"RecipientIdentity: {GetDisplayText()} (FP: {shortFingerprint}...)";
        }
    }
}