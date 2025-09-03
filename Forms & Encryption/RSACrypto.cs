using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace OffCrypt
{
    /// <summary>
    /// RSA-4096 encryption implementation with OAEP padding
    /// Compatible with Android NETCrypt RSA implementation
    /// </summary>
    public class RSACrypto
    {
        private const byte RSA_4096_VERSION = 0x0A;
        private const int RSA_4096_KEY_SIZE = 4096;
        
        public RSACrypto()
        {
        }
        
        /// <summary>
        /// Generates a new RSA-4096 key pair
        /// </summary>
        public RSAParameters[] GenerateRSA4096KeyPair()
        {
            try
            {
                using (var rsa = RSA.Create(RSA_4096_KEY_SIZE))
                {
                    // Generate new key pair
                    var privateKey = rsa.ExportParameters(true);  // Include private key
                    var publicKey = rsa.ExportParameters(false);  // Public key only
                    
                    return new RSAParameters[] { privateKey, publicKey };
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"RSA key pair generation failed: {ex.Message}", ex);
            }
        }
        
        /// <summary>
        /// Encrypts message using RSA-4096 with OAEP padding
        /// </summary>
        public string EncryptRSA4096(string message, RSAParameters publicKey)
        {
            try
            {
                if (string.IsNullOrEmpty(message))
                    throw new ArgumentException("Message cannot be null or empty");
                
                var messageBytes = Encoding.UTF8.GetBytes(message);
                
                using (var rsa = RSA.Create())
                {
                    rsa.ImportParameters(publicKey);
                    
                    // Use OAEP padding with SHA-256 (equivalent to Android implementation)
                    var encryptedData = rsa.Encrypt(messageBytes, RSAEncryptionPadding.OaepSHA256);
                    
                    // Add version byte and create output
                    using (var outputStream = new MemoryStream())
                    {
                        outputStream.WriteByte(RSA_4096_VERSION);
                        outputStream.Write(encryptedData, 0, encryptedData.Length);
                        
                        return Convert.ToBase64String(outputStream.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"RSA-4096 encryption failed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Encrypts message using RSA-4096 with custom version byte (for disappearing messages)
        /// </summary>
        public string EncryptRSA4096WithTimestamp(string message, RSAParameters publicKey, byte versionByte)
        {
            try
            {
                if (string.IsNullOrEmpty(message))
                    throw new ArgumentException("Message cannot be null or empty");
                
                var messageBytes = Encoding.UTF8.GetBytes(message);
                
                using (var rsa = RSA.Create())
                {
                    rsa.ImportParameters(publicKey);
                    
                    // Use OAEP padding with SHA-256
                    var encryptedData = rsa.Encrypt(messageBytes, RSAEncryptionPadding.OaepSHA256);
                    
                    // Add custom version byte and create output
                    using (var outputStream = new MemoryStream())
                    {
                        outputStream.WriteByte(versionByte); // Custom version for disappearing messages
                        outputStream.Write(encryptedData, 0, encryptedData.Length);
                        
                        return Convert.ToBase64String(outputStream.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"RSA-4096 timestamp encryption failed: {ex.Message}", ex);
            }
        }
        
        /// <summary>
        /// Decrypts RSA-4096 encrypted message
        /// </summary>
        public string DecryptRSA4096(string encryptedText, RSAParameters privateKey)
        {
            try
            {
                if (string.IsNullOrEmpty(encryptedText))
                    throw new ArgumentException("Encrypted text cannot be null or empty");
                
                var encryptedData = Convert.FromBase64String(encryptedText);
                
                if (encryptedData.Length < 2)
                    throw new ArgumentException("Invalid encrypted data size");
                
                // Check version byte
                var versionByte = encryptedData[0];
                if (versionByte != RSA_4096_VERSION)
                    throw new ArgumentException($"Invalid RSA version. Expected {RSA_4096_VERSION:X2}, got {versionByte:X2}");
                
                // Extract encrypted message (skip version byte)
                var encryptedMessage = new byte[encryptedData.Length - 1];
                Array.Copy(encryptedData, 1, encryptedMessage, 0, encryptedMessage.Length);
                
                using (var rsa = RSA.Create())
                {
                    rsa.ImportParameters(privateKey);
                    
                    // Decrypt with OAEP padding and SHA-256
                    var decryptedBytes = rsa.Decrypt(encryptedMessage, RSAEncryptionPadding.OaepSHA256);
                    
                    return Encoding.UTF8.GetString(decryptedBytes);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"RSA-4096 decryption failed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Decrypts RSA-4096 encrypted message with timestamp support
        /// </summary>
        public string DecryptRSA4096WithTimestamp(string encryptedText, RSAParameters privateKey)
        {
            try
            {
                if (string.IsNullOrEmpty(encryptedText))
                    throw new ArgumentException("Encrypted text cannot be null or empty");
                
                var encryptedData = Convert.FromBase64String(encryptedText);
                
                if (encryptedData.Length < 2)
                    throw new ArgumentException("Invalid encrypted data size");
                
                // Check version byte (accept both normal and disappearing versions)
                var versionByte = encryptedData[0];
                if (versionByte != RSA_4096_VERSION && versionByte != 0x0B) // Accept both normal and disappearing
                    throw new ArgumentException($"Invalid RSA version. Expected {RSA_4096_VERSION:X2} or 0B, got {versionByte:X2}");
                
                // Extract encrypted message (skip version byte)
                var encryptedMessage = new byte[encryptedData.Length - 1];
                Array.Copy(encryptedData, 1, encryptedMessage, 0, encryptedMessage.Length);
                
                using (var rsa = RSA.Create())
                {
                    rsa.ImportParameters(privateKey);
                    
                    // Decrypt with OAEP padding and SHA-256
                    var decryptedBytes = rsa.Decrypt(encryptedMessage, RSAEncryptionPadding.OaepSHA256);
                    
                    return Encoding.UTF8.GetString(decryptedBytes);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"RSA-4096 timestamp decryption failed: {ex.Message}", ex);
            }
        }
        
        /// <summary>
        /// Exports public key in PEM format for sharing
        /// </summary>
        public string ExportPublicKeyToPEM(RSAParameters publicKey)
        {
            try
            {
                using (var rsa = RSA.Create())
                {
                    rsa.ImportParameters(publicKey);
                    
                    // Use modern .NET 7 method to export as standard PEM format
                    var keyData = rsa.ExportSubjectPublicKeyInfo();
                    var base64Key = Convert.ToBase64String(keyData);
                    
                    // Format as standard PEM structure
                    var pemBuilder = new StringBuilder();
                    pemBuilder.AppendLine("-----BEGIN PUBLIC KEY-----");
                    
                    // Split into 64-character lines
                    for (int i = 0; i < base64Key.Length; i += 64)
                    {
                        var length = Math.Min(64, base64Key.Length - i);
                        pemBuilder.AppendLine(base64Key.Substring(i, length));
                    }
                    
                    pemBuilder.AppendLine("-----END PUBLIC KEY-----");
                    return pemBuilder.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Public key export failed: {ex.Message}", ex);
            }
        }
        
        /// <summary>
        /// Imports public key from PEM format
        /// </summary>
        public RSAParameters ImportPublicKeyFromPEM(string pemKey)
        {
            try
            {
                if (string.IsNullOrEmpty(pemKey))
                    throw new ArgumentException("PEM key cannot be null or empty");
                
                // Remove headers and whitespace
                var cleanKey = pemKey
                    .Replace("-----BEGIN PUBLIC KEY-----", "")
                    .Replace("-----END PUBLIC KEY-----", "")
                    .Replace("\r", "")
                    .Replace("\n", "")
                    .Replace(" ", "");
                
                var keyData = Convert.FromBase64String(cleanKey);
                
                using (var rsa = RSA.Create())
                {
                    // Use modern .NET 7 method to import standard PEM format
                    rsa.ImportSubjectPublicKeyInfo(keyData, out _);
                    return rsa.ExportParameters(false);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Public key import failed: {ex.Message}", ex);
            }
        }
        
        /// <summary>
        /// Gets the maximum message size for RSA-4096 with OAEP-SHA256 padding
        /// </summary>
        public int GetMaxMessageSize()
        {
            // RSA-4096 = 512 bytes, OAEP-SHA256 overhead = 66 bytes
            return (RSA_4096_KEY_SIZE / 8) - 66;
        }
        
        public void Dispose()
        {
            // No resources to dispose in modern implementation
        }
    }
}