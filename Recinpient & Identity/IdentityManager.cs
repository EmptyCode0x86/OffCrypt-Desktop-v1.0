using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Text;

namespace OffCrypt
{
    public class IdentityManager
    {
        private readonly Form _parentForm;
        private readonly ModernTextBox _currentIdentityTextBox;

        public IdentityManager(Form parentForm, ModernTextBox currentIdentityTextBox)
        {
            _parentForm = parentForm;
            _currentIdentityTextBox = currentIdentityTextBox;
        }

        public void CheckAndLoadIdentityOnStartup()
        {
            try
            {
                // Check if identities exist
                if (!Directory.Exists(KeyringManager.KeyringDir))
                {
                    ShowFirstTimeIdentityPrompt();
                    return;
                }

                var pubFiles = Directory.GetFiles(KeyringManager.KeyringDir, "*.ocpub");
                if (pubFiles.Length == 0)
                {
                    ShowFirstTimeIdentityPrompt();
                    return;
                }

                // Identity found, load it
                LoadCurrentIdentity();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Identity check failed on startup: {ex.Message}");
                _currentIdentityTextBox.Text = "Error checking identity. Use 'Create new identity' button to create one.";
            }
        }

        public void LoadCurrentIdentity()
        {
            try
            {
                // Check if identities exist
                if (!Directory.Exists(KeyringManager.KeyringDir))
                {
                    _currentIdentityTextBox.Text = "No identity found.\n\n" +
                                                  "Create your first identity\n" +
                                                  "using the button above\n" +
                                                  "to get started.";
                    return;
                }

                var pubFiles = Directory.GetFiles(KeyringManager.KeyringDir, "*.ocpub");
                if (pubFiles.Length == 0)
                {
                    _currentIdentityTextBox.Text = "No identity found.\n\n" +
                                                  "Create your first identity\n" +
                                                  "using the button above\n" +
                                                  "to get started.";
                    return;
                }

                // Load first available identity
                var pubFile = pubFiles[0];
                var fileName = Path.GetFileNameWithoutExtension(pubFile);

                // Try to load encrypted metadata first
                var encryptedMetadata = IdentityMetadata.LoadEncrypted(fileName);
                if (encryptedMetadata != null)
                {
                    // Update LastUsed timestamp
                    encryptedMetadata.UpdateLastUsed();

                    var identityText = string.IsNullOrEmpty(encryptedMetadata.DisplayName)
                        ? $"Random ID:\n{encryptedMetadata.RandomId}"
                        : $"Name:\n{encryptedMetadata.DisplayName}\n\nRandom ID:\n{encryptedMetadata.RandomId}";

                    _currentIdentityTextBox.Text = $"Current Identity:\n\n" +
                                                  $"{identityText}\n\n" +
                                                  $"Fingerprint:\n{encryptedMetadata.FingerprintHex[..16]}...\n\n" +
                                                  $"Created:\n{encryptedMetadata.CreatedUtc:yyyy-MM-dd}\n\n" +
                                                  $"Last Used:\n{encryptedMetadata.LastUsed:yyyy-MM-dd HH:mm}";
                }
                else
                {
                    // Improved fallback: parse the public key properly to extract Random ID
                    var (randomId, displayName) = ParseRandomIdFromPublicKeyFile(pubFile, fileName);
                    
                    if (!string.IsNullOrEmpty(randomId))
                    {
                        var identityText = string.IsNullOrEmpty(displayName)
                            ? $"Random ID:\n{randomId}"
                            : $"Name:\n{displayName}\n\nRandom ID:\n{randomId}";
                            
                        _currentIdentityTextBox.Text = $"Current Identity:\n\n" +
                                                      $"{identityText}\n\n" +
                                                      $"Fingerprint:\n{fileName[..Math.Min(16, fileName.Length)]}...\n\n" +
                                                      $"⚠️ Metadata not encrypted\n- upgrade recommended";
                    }
                    else
                    {
                        // Ultimate fallback: show basic info from fingerprint
                        var shortId = fileName.Length >= 8 ? fileName.Substring(0, 8) : fileName;
                        _currentIdentityTextBox.Text = $"Current Identity:\n\n" +
                                                      $"Random ID:\n{shortId.ToUpper()}\n\n" +
                                                      $"Fingerprint:\n{fileName[..Math.Min(16, fileName.Length)]}...\n\n" +
                                                      $"⚠️ Metadata not encrypted\n- upgrade recommended";
                    }
                }
            }
            catch (Exception ex)
            {
                _currentIdentityTextBox.Text = $"Error loading identity: {ex.Message}";
            }
        }

        public void OpenIdentityRegisterForm()
        {
            try
            {
                // Open IdentityRegisterForm to create new identity
                using (var identityForm = new IdentityRegisterForm())
                {
                    identityForm.ShowDialog(_parentForm);
                    
                    // Update identity when returning
                    LoadCurrentIdentity();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Identity form opening failed: {ex.Message}",
                               "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public bool HasExistingIdentity()
        {
            try
            {
                if (!Directory.Exists(KeyringManager.KeyringDir))
                    return false;

                var pubFiles = Directory.GetFiles(KeyringManager.KeyringDir, "*.ocpub");
                return pubFiles.Length > 0;
            }
            catch
            {
                return false;
            }
        }

        private void ShowFirstTimeIdentityPrompt()
        {
            // Set instructions in text field
            _currentIdentityTextBox.Text = "Welcome to OffCrypt!\n\n" +
                                          "No identity found.\n\n" +
                                          "Please create your first\n" +
                                          "identity using the\n" +
                                          "'Create new identity'\n" +
                                          "button above to get started.";

            // Show friendly welcome message
            _parentForm.BeginInvoke(new Action(() =>
            {
                var result = MessageBox.Show(
                    "Welcome to OffCrypt!\n\n" +
                    "To use OffCrypt, you need to create a PGP identity first.\n\n" +
                    "Would you like to create your identity now?",
                    "Create Identity",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information);

                if (result == DialogResult.Yes)
                {
                    // Open IdentityRegisterForm automatically
                    OpenIdentityRegisterForm();
                }
            }));
        }

        /// <summary>
        /// Gets current user's identity information for message headers
        /// Returns (RandomId, DisplayName) or empty strings if no identity found
        /// </summary>
        public (string randomId, string displayName) GetCurrentUserIdentity()
        {
            try
            {
                if (!Directory.Exists(KeyringManager.KeyringDir))
                    return ("", "");

                var pubFiles = Directory.GetFiles(KeyringManager.KeyringDir, "*.ocpub");
                if (pubFiles.Length == 0)
                    return ("", "");

                // Get first identity (could be enhanced to support multiple identities later)
                var pubFile = pubFiles[0];
                var fileName = Path.GetFileNameWithoutExtension(pubFile);

                // Try to load encrypted metadata first
                var encryptedMetadata = IdentityMetadata.LoadEncrypted(fileName);
                if (encryptedMetadata != null)
                {
                    return (encryptedMetadata.RandomId, encryptedMetadata.DisplayName);
                }

                // Improved fallback: parse the public key properly to extract Random ID
                return ParseRandomIdFromPublicKeyFile(pubFile, fileName);
            }
            catch
            {
                return ("", "");
            }
        }

        /// <summary>
        /// Parses Random ID from .ocpub file safely
        /// Uses same logic as KeyRingManager, but without reflection
        /// </summary>
        private (string randomId, string displayName) ParseRandomIdFromPublicKeyFile(string pubFilePath, string fileName)
        {
            try
            {
                // Read .ocpub file
                string pubPemContent = File.ReadAllText(pubFilePath, Encoding.UTF8);
                
                // Extract userId from Comment line (same logic as KeyRingManager.ExtractUserIdFromPem)
                string userId = ExtractUserIdFromPemContent(pubPemContent);
                
                if (!string.IsNullOrEmpty(userId))
                {
                    // Parse userId and return (randomId, displayName)
                    return ParseUserIdString(userId);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to parse public key file properly: {ex.Message}");
            }

            // Ultimate fallback: use filename as before
            var shortId = fileName.Length >= 8 ? fileName.Substring(0, 8) : fileName;
            return (shortId.ToUpper(), "");
        }

        /// <summary>
        /// Extract userId from PEM content (same logic as KeyRingManager.ExtractUserIdFromPem)
        /// </summary>
        private static string ExtractUserIdFromPemContent(string pemContent)
        {
            try
            {
                // Look for comment line that might contain userId information
                var lines = pemContent.Split('\n');
                foreach (var line in lines)
                {
                    if (line.StartsWith("Comment:") && line.Contains("[") && line.Contains("]"))
                    {
                        // Find RandomID from brackets
                        int start = line.LastIndexOf('[');
                        int end = line.LastIndexOf(']');
                        if (start >= 0 && end > start)
                        {
                            string randomId = line.Substring(start + 1, end - start - 1);
                            string displayName = "";
                            
                            // If comment has name before brackets
                            string beforeBrackets = line.Substring(line.IndexOf(':') + 1, start - line.IndexOf(':') - 1).Trim();
                            if (!string.IsNullOrWhiteSpace(beforeBrackets))
                            {
                                displayName = beforeBrackets;
                            }
                            
                            return string.IsNullOrEmpty(displayName) ? randomId : $"{displayName} [{randomId}]";
                        }
                    }
                }
                
                return ""; // No userId information found
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// Parse userId string and return (randomId, displayName) (same logic as KeyRingManager.ParseUserId)
        /// </summary>
        private static (string randomId, string displayName) ParseUserIdString(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return ("", "");

            // If format is "DisplayName [A7F3B8C2]"
            var match = System.Text.RegularExpressions.Regex.Match(userId, @"^(.+?)\s*\[([A-F0-9]{8})\]$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            if (match.Success)
                return (match.Groups[2].Value, match.Groups[1].Value.Trim());

            // If only randomId (8 hex characters)
            if (System.Text.RegularExpressions.Regex.IsMatch(userId, @"^[A-F0-9]{8}$", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                return (userId, "");

            // Otherwise use entire string as displayName
            return ("", userId);
        }
    }
}