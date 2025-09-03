using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace OffCrypt
{
    /// <summary>
    /// X25519Manager handles all X25519 cryptographic operations in OffCryptDesktop
    /// Replaces the old RSAOperations class and provides modern X25519-based encryption
    /// </summary>
    public class X25519Manager : IDisposable
    {
        // UI References
        private readonly TextBox publicKeyTextBox;
        private readonly TextBox writeMessageTextBox;
        private readonly TextBox decryptMessageTextBox;
        private readonly Func<char[]> getCurrentPassword;
        
        // Disappearing messages UI references
        private readonly ModernTextBox dispX255txtbox;
        private readonly ModernCheckBox hourX255Chck;
        private readonly ModernCheckBox dayX255Chck;
        private readonly ModernCheckBox weekX255Chck;
        private readonly ModernCheckBox monthX255Chck;
        private readonly ModernToggleSwitch toggleDispX255;

        // Key storage
        private byte[]? currentPrivateKey = null;
        private byte[]? currentPublicKey = null;
        private byte[]? recipientPublicKey = null;

        // Session management
        private byte[] currentSessionId = null!;
        private byte[] currentSenderId = null!;
        private uint messageSequence = 0;

        // Registry keys
        private const string REGISTRY_KEY = @"SOFTWARE\OffCrypt\X25519";
        private const string PRIVATE_KEY_VALUE = "PrivateKey";
        private const string PUBLIC_KEY_VALUE = "PublicKey";
        private const string SESSION_ID_VALUE = "SessionId";
        private const string SENDER_ID_VALUE = "SenderId";

        public X25519Manager(
            TextBox publicKeyTextBox, 
            TextBox writeMessageTextBox, 
            TextBox decryptMessageTextBox,
            Func<char[]> passwordProvider,
            ModernTextBox dispX255txtbox = null,
            ModernCheckBox hourX255Chck = null,
            ModernCheckBox dayX255Chck = null,
            ModernCheckBox weekX255Chck = null,
            ModernCheckBox monthX255Chck = null,
            ModernToggleSwitch toggleDispX255 = null)
        {
            this.publicKeyTextBox = publicKeyTextBox;
            this.writeMessageTextBox = writeMessageTextBox;
            this.decryptMessageTextBox = decryptMessageTextBox;
            this.getCurrentPassword = passwordProvider;
            
            // Disappearing messages UI components (optional)
            this.dispX255txtbox = dispX255txtbox;
            this.hourX255Chck = hourX255Chck;
            this.dayX255Chck = dayX255Chck;
            this.weekX255Chck = weekX255Chck;
            this.monthX255Chck = monthX255Chck;
            this.toggleDispX255 = toggleDispX255;

            // Load existing keys and session information
            LoadStoredKeys();
            InitializeSession();
        }

        /// <summary>
        /// Creates new X25519 key pair and saves them
        /// </summary>
        public void GenerateKeyPair()
        {
            try
            {
                // Create new key pair
                var (privateKeyDer, publicKeyDer) = X25519Util.Generate();
                currentPrivateKey = privateKeyDer;
                currentPublicKey = publicKeyDer;

                // Create new session
                InitializeSession();

                // Save keys to registry (encrypted with DPAPI)
                SaveKeysToRegistry();

                // Display public key in UI
                DisplayPublicKey();

                MessageBox.Show(
                    "✅ X25519 key pair created successfully!\n\n" +
                    "🔐 Private key: protected with Windows DPAPI\n" +
                    "🔑 Public key: visible above and ready for sharing\n\n" +
                    "Your public key is now ready for use.",
                    "Key pair created",
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Key pair creation failed: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Encrypts message and shows encrypted result
        /// </summary>
        public void EncryptAndShowMessage()
        {
            try
            {
                string message = writeMessageTextBox.Text.Trim();
                
                if (string.IsNullOrEmpty(message))
                {
                    MessageBox.Show("Enter message first!", "Error",
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (currentPrivateKey == null || recipientPublicKey == null)
                {
                    MessageBox.Show("Keys missing! Create key pair and import recipient's public key.",
                                   "Keys missing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Check if disappearing messages is enabled
                string encryptedBase64;
                bool isDisappearing = false;
                
                try
                {
                    // Try to get disappearing messages settings
                    var disappearingSettings = DisappearingMessages.SettingsManager.GetRSASettings(
                        dispX255txtbox,
                        hourX255Chck,
                        dayX255Chck,
                        weekX255Chck,
                        monthX255Chck,
                        toggleDispX255);
                    
                    if (disappearingSettings != null && disappearingSettings.IsEnabled)
                    {
                        // Create disappearing message
                        encryptedBase64 = DisappearingMessages.CreateDisappearingX25519Message(
                            message,
                            currentPrivateKey,
                            recipientPublicKey,
                            currentSessionId,
                            currentSenderId,
                            messageSequence++,
                            disappearingSettings.ExpireAfter);
                        isDisappearing = true;
                    }
                    else
                    {
                        // Regular X25519 encryption
                        var plaintextBytes = Encoding.UTF8.GetBytes(message);
                        var encryptedPacket = X25519StatelessSession.InitiatorEncrypt(
                            currentPrivateKey,
                            recipientPublicKey,
                            currentSessionId,
                            currentSenderId,
                            messageSequence++,
                            plaintextBytes);

                        encryptedBase64 = Convert.ToBase64String(encryptedPacket);
                    }
                }
                catch (Exception ex)
                {
                    // If disappearing messages fails, fall back to regular encryption
                    var plaintextBytes = Encoding.UTF8.GetBytes(message);
                    var encryptedPacket = X25519StatelessSession.InitiatorEncrypt(
                        currentPrivateKey,
                        recipientPublicKey,
                        currentSessionId,
                        currentSenderId,
                        messageSequence++,
                        plaintextBytes);

                    encryptedBase64 = Convert.ToBase64String(encryptedPacket);
                }

                writeMessageTextBox.Text = encryptedBase64;

                // Copy to clipboard
                Clipboard.SetText(encryptedBase64);

                string messageInfo = isDisappearing ? 
                    "✅ Message encrypted with X25519 + Disappearing Messages!\n\n" :
                    "✅ Message encrypted with X25519 algorithm!\n\n";
                
                MessageBox.Show(
                    messageInfo +
                    $"📦 Message size: {encryptedBase64.Length} characters\n" +
                    $"📋 Copied to clipboard\n" +
                    $"🔢 Message number: {messageSequence - 1}\n" +
                    (isDisappearing ? "⏰ Message will disappear after expiration!" : "🔒 Regular encrypted message"),
                    "Encryption successful",
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Encryption failed: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Import and decrypt encrypted message
        /// </summary>
        public void ImportAndDecryptMessage()
        {
            try
            {
                if (currentPrivateKey == null)
                {
                    MessageBox.Show("Private key missing! Create key pair first.",
                                   "Key missing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Get encrypted message from clipboard or UI
                string encryptedText = "";
                if (Clipboard.ContainsText())
                {
                    encryptedText = Clipboard.GetText().Trim();
                }
                
                if (string.IsNullOrEmpty(encryptedText))
                {
                    MessageBox.Show("Copy encrypted message to clipboard first!", "No message",
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Decode Base64
                byte[] encryptedPacket;
                try
                {
                    encryptedPacket = Convert.FromBase64String(encryptedText);
                }
                catch
                {
                    MessageBox.Show("Invalid Base64 format!", "Error",
                                   MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Check if this is a disappearing message and decrypt accordingly
                string decryptedMessage;
                bool isDisappearing = DisappearingMessages.IsDisappearingMessage(encryptedText);
                
                try
                {
                    if (isDisappearing)
                    {
                        // Decrypt disappearing message (handles expiration automatically)
                        decryptedMessage = DisappearingMessages.DecryptDisappearingX25519Message(
                            encryptedText,
                            currentPrivateKey);
                    }
                    else
                    {
                        // Regular X25519 decryption
                        var decryptedBytes = X25519StatelessSession.RecipientDecrypt(
                            currentPrivateKey,
                            encryptedPacket,
                            null, // no extraAad
                            out uint seq,
                            out byte[] sessionId,
                            out byte[] initiatorPubKey,
                            out byte[] senderId);

                        decryptedMessage = Encoding.UTF8.GetString(decryptedBytes);
                    }
                }
                catch (MessageExpiredException ex)
                {
                    // Handle expired messages specifically
                    MessageBox.Show(ex.Message, "Message Expired", 
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    decryptMessageTextBox.Text = "❌ Message has expired and was destroyed";
                    return;
                }

                decryptMessageTextBox.Text = decryptedMessage;

                // Show information (different for disappearing vs regular messages)
                string messageInfo;
                if (isDisappearing)
                {
                    messageInfo = $"✅ Disappearing message decrypted successfully!\n\n" +
                                 $"⏰ Message type: X25519 Disappearing\n" +
                                 $"📝 Message length: {decryptedMessage.Length} characters\n" +
                                 $"⚠️ This message may expire soon!";
                }
                else
                {
                    // For regular messages, we have access to sender info
                    var decryptedBytes = X25519StatelessSession.RecipientDecrypt(
                        currentPrivateKey,
                        encryptedPacket,
                        null, // no extraAad
                        out uint seq,
                        out byte[] sessionId,
                        out byte[] initiatorPubKey,
                        out byte[] senderId);
                    
                    var senderFingerprint = Convert.ToHexString(X25519Util.Fingerprint16(initiatorPubKey))[..8];
                    messageInfo = $"✅ Message decrypted successfully!\n\n" +
                                 $"👤 Sender: {senderFingerprint}\n" +
                                 $"🔢 Message number: {seq}\n" +
                                 $"📝 Message length: {decryptedMessage.Length} characters";
                }
                
                MessageBox.Show(messageInfo, "Decryption successful",
                               MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Message decryption failed: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
                decryptMessageTextBox.Text = "Decryption failed";
            }
        }

        /// <summary>
        /// Import recipient's public key in PEM format
        /// </summary>
        public void ImportRecipientPublicKey(string publicKeyPem)
        {
            try
            {
                recipientPublicKey = X25519Util.FromPem(publicKeyPem, "PUBLIC KEY");
                var fingerprint = Convert.ToHexString(X25519Util.Fingerprint16(recipientPublicKey))[..8];
                
                MessageBox.Show(
                    $"✅ Recipient's public key imported!\n\n" +
                    $"🔑 Fingerprint: {fingerprint}\n" +
                    $"📏 Size: {recipientPublicKey.Length} bytes\n\n" +
                    "You can now send encrypted messages to this recipient.",
                    "Key imported",
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Public key import failed: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Copy own public key to clipboard
        /// </summary>
        public void CopyMyPublicKeyToClipboard()
        {
            try
            {
                if (currentPublicKey == null)
                {
                    MessageBox.Show("Public key missing! Create key pair first.", "Key missing",
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var publicKeyPem = X25519Util.ToPemPublic(currentPublicKey);
                Clipboard.SetText(publicKeyPem);
                
                MessageBox.Show("Public key copied to clipboard!", "Copied",
                               MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Copy failed: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Show public key information
        /// </summary>
        public void ShowPublicKeyInfo()
        {
            try
            {
                if (currentPublicKey == null)
                {
                    MessageBox.Show("Public key missing!", "No key",
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var fingerprint = Convert.ToHexString(X25519Util.Fingerprint16(currentPublicKey));
                var shortFingerprint = fingerprint[..8];

                MessageBox.Show(
                    $"🔑 X25519 Public Key Information\n\n" +
                    $"📏 Size: {currentPublicKey.Length} bytes\n" +
                    $"🔐 Algorithm: X25519 (Curve25519)\n" +
                    $"👤 Fingerprint: {shortFingerprint}\n" +
                    $"📋 Full fingerprint:\n{fingerprint}\n\n" +
                    $"🛡️ Security level: 128-bit (equivalent to RSA-3072)\n" +
                    $"⚡ Performance: ~100x faster than RSA-4096",
                    "Key information",
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Showing key information failed: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Check if public key exists
        /// </summary>
        public bool HasPublicKey()
        {
            return currentPublicKey != null;
        }

        /// <summary>
        /// Check if private key exists
        /// </summary>
        public bool HasPrivateKey()
        {
            return currentPrivateKey != null;
        }

        /// <summary>
        /// Check if recipient's public key has been imported
        /// </summary>
        public bool HasRecipientKey()
        {
            return recipientPublicKey != null;
        }

        /// <summary>
        /// Returns current public key in PEM format
        /// </summary>
        public string? GetCurrentPublicKeyPEM()
        {
            if (currentPublicKey == null) return null;
            return X25519Util.ToPemPublic(currentPublicKey);
        }

        #region Private Methods

        private void LoadStoredKeys()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(REGISTRY_KEY);
                if (key == null) return;

                // Load private key (DPAPI protected)
                if (key.GetValue(PRIVATE_KEY_VALUE) is byte[] encryptedPrivateKey)
                {
                    try
                    {
                        currentPrivateKey = ProtectedData.Unprotect(encryptedPrivateKey, null, DataProtectionScope.CurrentUser);
                    }
                    catch
                    {
                        // Key corrupted or no access
                        currentPrivateKey = null;
                    }
                }

                // Load public key
                if (key.GetValue(PUBLIC_KEY_VALUE) is byte[] publicKey)
                {
                    currentPublicKey = publicKey;
                }

                // Load session information
                if (key.GetValue(SESSION_ID_VALUE) is byte[] sessionId && sessionId.Length == 16)
                {
                    currentSessionId = sessionId;
                }

                if (key.GetValue(SENDER_ID_VALUE) is byte[] senderId && senderId.Length == 16)
                {
                    currentSenderId = senderId;
                }
            }
            catch
            {
                // Ignore registry errors
            }
        }

        private void SaveKeysToRegistry()
        {
            try
            {
                using var key = Registry.CurrentUser.CreateSubKey(REGISTRY_KEY);
                
                // Save private key (DPAPI protected)
                if (currentPrivateKey != null)
                {
                    var encryptedPrivateKey = ProtectedData.Protect(currentPrivateKey, null, DataProtectionScope.CurrentUser);
                    key.SetValue(PRIVATE_KEY_VALUE, encryptedPrivateKey, RegistryValueKind.Binary);
                }

                // Save public key
                if (currentPublicKey != null)
                {
                    key.SetValue(PUBLIC_KEY_VALUE, currentPublicKey, RegistryValueKind.Binary);
                }

                // Save session information
                if (currentSessionId != null)
                {
                    key.SetValue(SESSION_ID_VALUE, currentSessionId, RegistryValueKind.Binary);
                }

                if (currentSenderId != null)
                {
                    key.SetValue(SENDER_ID_VALUE, currentSenderId, RegistryValueKind.Binary);
                }
            }
            catch
            {
                // Ignore registry errors
            }
        }

        private void InitializeSession()
        {
            // Create new session ID if doesn't exist
            if (currentSessionId == null || currentSessionId.Length != 16)
            {
                currentSessionId = Rng.Bytes(16);
            }

            // Create sender ID from public key if possible
            if (currentPublicKey != null)
            {
                currentSenderId = X25519Util.Fingerprint16(currentPublicKey);
            }
            else if (currentSenderId == null || currentSenderId.Length != 16)
            {
                currentSenderId = Rng.Bytes(16);
            }

            // Save session information
            SaveKeysToRegistry();
        }

        private void DisplayPublicKey()
        {
            if (currentPublicKey != null)
            {
                var publicKeyPem = X25519Util.ToPemPublic(currentPublicKey);
                publicKeyTextBox.Text = publicKeyPem;
            }
        }

        #endregion

        public void Dispose()
        {
            // Clear sensitive data from memory
            if (currentPrivateKey != null)
            {
                Array.Clear(currentPrivateKey, 0, currentPrivateKey.Length);
                currentPrivateKey = null;
            }

            if (recipientPublicKey != null)
            {
                Array.Clear(recipientPublicKey, 0, recipientPublicKey.Length);
                recipientPublicKey = null;
            }

            if (currentSessionId != null)
            {
                Array.Clear(currentSessionId, 0, currentSessionId.Length);
                currentSessionId = null;
            }

            if (currentSenderId != null)
            {
                Array.Clear(currentSenderId, 0, currentSenderId.Length);
                currentSenderId = null;
            }
        }
    }
}