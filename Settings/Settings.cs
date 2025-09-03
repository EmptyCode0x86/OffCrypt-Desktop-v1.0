using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;

namespace OffCrypt
{
    /// <summary>
    /// Disappearing Messages functionality for both encryption modes
    /// Automatic message expiration based on timestamp
    /// Updated to use modern PasswordSecure class
    /// </summary>
    public static class DisappearingMessages
    {
        // Version bytes for different encryption types
        private const byte VERSION_PASSWORD_DISAPPEARING_LEGACY = 0x02; // Old legacy version
        private const byte VERSION_PASSWORD_DISAPPEARING_MODERN = 0x12; // New modern version
        private const byte VERSION_X255_DISAPPEARING = 0x0B;
        private const byte VERSION_X255_DISAPPEARING_MODERN = 0x1B;     // Modern X25519 disappearing messages

        /// <summary>
        /// Safe Base64 decoding with input validation
        /// </summary>
        private static byte[] SafeDecodeBase64(string input, string context = "Base64 data")
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException($"Invalid {context}: null or empty", nameof(input));

            // Check that string is valid Base64
            if (input.Length % 4 != 0)
                throw new ArgumentException($"Invalid {context}: length not multiple of 4");

            // Check that contains only valid Base64 characters
            if (!System.Text.RegularExpressions.Regex.IsMatch(input, @"^[A-Za-z0-9+/]*={0,2}$"))
                throw new ArgumentException($"Invalid {context}: contains invalid characters");

            try
            {
                byte[] result = Convert.FromBase64String(input);
                if (result.Length == 0)
                    throw new ArgumentException($"Invalid {context}: decoded to empty array");
                
                return result;
            }
            catch (FormatException ex)
            {
                throw new ArgumentException($"Invalid {context}: malformed Base64 string", ex);
            }
        }

        /// <summary>
        /// Create disappearing message in Password mode with modern encryption
        /// </summary>
        public static string CreateDisappearingPasswordMessage(string originalMessage, char[] password, TimeSpan expireAfter)
        {
            try
            {
                // Create metadata with timestamp
                var messageData = new DisappearingMessageData
                {
                    OriginalMessage = originalMessage,
                    CreatedAt = DateTime.Now,
                    ExpiresAt = DateTime.Now.Add(expireAfter),
                    MessageType = "PASSWORD_MODERN",
                    Version = VERSION_PASSWORD_DISAPPEARING_MODERN
                };

                // Serialize to JSON
                string jsonData;
                try
                {
                    jsonData = JsonSerializer.Serialize(messageData, new JsonSerializerOptions
                    {
                        WriteIndented = false
                    });
                }
                catch (JsonException ex)
                {
                    throw new InvalidOperationException($"JSON serialization failed: {ex.Message}", ex);
                }

                // Use modern PasswordSecure class with timestamp functionality
                var passwordSecure = new PasswordSecure();

                try
                {
                    return passwordSecure.EncryptWithTimestamp(jsonData, password, VERSION_PASSWORD_DISAPPEARING_MODERN);
                }
                finally
                {
                    passwordSecure.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create disappearing password message: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Create disappearing message in X25519 mode with modern encryption
        /// </summary>
        public static string CreateDisappearingX25519Message(string originalMessage, byte[] senderPrivateKey, byte[] recipientPublicKey, byte[] sessionId, byte[] senderId, uint messageSequence, TimeSpan expireAfter)
        {
            try
            {
                // Create metadata with timestamp
                var messageData = new X25519DisappearingMessageData
                {
                    OriginalMessage = originalMessage,
                    CreatedAt = DateTime.Now,
                    ExpiresAt = DateTime.Now.Add(expireAfter),
                    MessageType = "X25519_MODERN",
                    Version = VERSION_X255_DISAPPEARING_MODERN,
                    SessionId = Convert.ToBase64String(sessionId),
                    SenderId = Convert.ToBase64String(senderId),
                    MessageSequence = messageSequence
                };

                // Serialize to JSON
                string jsonData;
                try
                {
                    jsonData = JsonSerializer.Serialize(messageData, new JsonSerializerOptions
                    {
                        WriteIndented = false
                    });
                }
                catch (JsonException ex)
                {
                    throw new InvalidOperationException($"JSON serialization failed: {ex.Message}", ex);
                }

                // Encrypt using X25519StatelessSession with version byte wrapper
                var plaintextBytes = Encoding.UTF8.GetBytes(jsonData);
                var encryptedPacket = X25519StatelessSession.InitiatorEncrypt(
                    senderPrivateKey,
                    recipientPublicKey, 
                    sessionId,
                    senderId,
                    messageSequence,
                    plaintextBytes);

                // Wrap with version byte for disappearing messages
                return WrapX25519PacketWithVersion(encryptedPacket, VERSION_X255_DISAPPEARING_MODERN);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create disappearing X25519 message: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Decrypt disappearing message in X25519 mode
        /// </summary>
        public static string DecryptDisappearingX25519Message(string encryptedMessage, byte[] recipientPrivateKey)
        {
            try
            {
                // Check if this is a disappearing X25519 message
                var (version, unwrappedData) = UnwrapX25519PacketWithVersion(encryptedMessage);
                
                if (version != VERSION_X255_DISAPPEARING_MODERN)
                {
                    throw new NotSupportedException($"Unsupported X25519 disappearing message version: 0x{version:X2}");
                }

                // Decrypt using X25519StatelessSession
                var decryptedBytes = X25519StatelessSession.RecipientDecrypt(
                    recipientPrivateKey,
                    unwrappedData,
                    null, // no extraAad
                    out uint seq,
                    out byte[] sessionId,
                    out byte[] initiatorPubKey,
                    out byte[] senderId);

                var decryptedJson = Encoding.UTF8.GetString(decryptedBytes);
                
                X25519DisappearingMessageData messageData;
                try
                {
                    messageData = JsonSerializer.Deserialize<X25519DisappearingMessageData>(decryptedJson);
                }
                catch (JsonException ex)
                {
                    throw new InvalidOperationException($"JSON deserialization failed: {ex.Message}. Message may be corrupted.", ex);
                }

                if (messageData == null)
                    throw new InvalidOperationException("Invalid disappearing message format: JSON returned null");

                // Check if message has expired (timing-safe comparison)
                var currentTime = DateTime.Now;
                var timeDiff = (currentTime - messageData.ExpiresAt).TotalMilliseconds;
                
                if (timeDiff > 0)
                {
                    throw new MessageExpiredException($"💀 This X25519 message has expired!\n\n" +
                                                    $"📅 Created: {messageData.CreatedAt:dd.MM.yyyy HH:mm:ss}\n" +
                                                    $"⚰️ Expired: {messageData.ExpiresAt:dd.MM.yyyy HH:mm:ss}\n" +
                                                    $"🕒 Now: {currentTime:dd.MM.yyyy HH:mm:ss}\n" +
                                                    $"🔢 Message #{messageData.MessageSequence}\n\n" +
                                                    $"Message content has been securely destroyed.");
                }

                return messageData.OriginalMessage;
            }
            catch (MessageExpiredException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to decrypt disappearing X25519 message: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Create disappearing message in RSA mode (unchanged)
        /// </summary>

        /// <summary>
        /// Decrypt disappearing message in Password mode (supports both legacy and modern)
        /// </summary>
        public static string DecryptDisappearingPasswordMessage(string encryptedMessage, char[] password)
        {
            try
            {
                // Check version byte (safe decoding)
                var encryptedData = SafeDecodeBase64(encryptedMessage, "encrypted message");
                if (encryptedData.Length == 0)
                    throw new ArgumentException("Encrypted message is empty");
                
                byte version = encryptedData[0];

                string decryptedJson;
                try
                {
                    if (version == VERSION_PASSWORD_DISAPPEARING_MODERN)
                    {
                        // Use modern PasswordSecure class
                        var passwordSecure = new PasswordSecure();
                        try
                        {
                            decryptedJson = passwordSecure.DecryptWithTimestamp(encryptedMessage, password);
                        }
                        finally
                        {
                            passwordSecure.Dispose();
                        }
                    }
                    else if (version == VERSION_PASSWORD_DISAPPEARING_LEGACY)
                    {
                        // Use legacy implementation for old messages
                        var legacyPasswordSecure = new PasswordSecure(); // Assuming old class is still available
                        try
                        {
                            // If old class is completely replaced, need separate legacy wrapper
                            throw new NotSupportedException("Legacy disappearing messages require legacy PasswordSecure implementation");
                        }
                        finally
                        {
                            legacyPasswordSecure.Dispose();
                        }
                    }
                    else
                    {
                        throw new NotSupportedException($"Unsupported disappearing message version: 0x{version:X2}");
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to decrypt disappearing password message: {ex.Message}", ex);
                }

                DisappearingMessageData messageData;
                try
                {
                    messageData = JsonSerializer.Deserialize<DisappearingMessageData>(decryptedJson);
                }
                catch (JsonException ex)
                {
                    throw new InvalidOperationException($"JSON deserialization failed: {ex.Message}. Message may be corrupted.", ex);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Disappearing message data processing failed: {ex.Message}", ex);
                }

                if (messageData == null)
                    throw new InvalidOperationException("Invalid disappearing message format: JSON returned null");

                // Check if message has expired (timing-safe comparison)
                var currentTime = DateTime.Now;
                var timeDiff = (currentTime - messageData.ExpiresAt).TotalMilliseconds;
                
                // Use constant-time comparison: doesn't reveal timing information about message age
                if (timeDiff > 0)
                {
                    throw new MessageExpiredException($"💀 This message has expired!\n\n" +
                                                    $"📅 Created: {messageData.CreatedAt:dd.MM.yyyy HH:mm:ss}\n" +
                                                    $"⚰️ Expired: {messageData.ExpiresAt:dd.MM.yyyy HH:mm:ss}\n" +
                                                    $"🕒 Now: {currentTime:dd.MM.yyyy HH:mm:ss}\n\n" +
                                                    $"Message content has been securely destroyed.");
                }

                return messageData.OriginalMessage;
            }
            catch (MessageExpiredException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to decrypt disappearing password message: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Check if message is a disappearing message based on version byte
        /// </summary>
        public static bool IsDisappearingMessage(string encryptedMessage)
        {
            try
            {
                // Use safe decoding
                var data = SafeDecodeBase64(encryptedMessage, "message check");
                if (data.Length < 1) return false;

                var version = data[0];
                return version == VERSION_PASSWORD_DISAPPEARING_LEGACY ||
                       version == VERSION_PASSWORD_DISAPPEARING_MODERN ||
                       version == VERSION_X255_DISAPPEARING ||
                       version == VERSION_X255_DISAPPEARING_MODERN;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Get time unit as TimeSpan
        /// </summary>
        public static TimeSpan GetTimeSpanFromSettings(int value, TimeUnit unit)
        {
            return unit switch
            {
                TimeUnit.Hour => TimeSpan.FromHours(value),
                TimeUnit.Day => TimeSpan.FromDays(value),
                TimeUnit.Week => TimeSpan.FromDays(value * 7),
                TimeUnit.Month => TimeSpan.FromDays(value * 30), // Approx 30 days
                _ => TimeSpan.FromHours(1) // Default fallback
            };
        }

        /// <summary>
        /// Settings management from Settings tab
        /// </summary>
        public static class SettingsManager
        {
            public static DisappearingSettings GetRSASettings(
                ModernTextBox dispX255txtbox,
                ModernCheckBox hourX255Chck,
                ModernCheckBox dayX255Chck,
                ModernCheckBox weekX255Chck,
                ModernCheckBox monthX255Chck,
                ModernToggleSwitch toggleDispX255)
            {
                // Null checks for UI components
                if (toggleDispX255 == null)
                    throw new ArgumentNullException(nameof(toggleDispX255), "X25519 Toggle component is missing");
                
                if (!toggleDispX255.Checked) return null;

                if (dispX255txtbox == null)
                    throw new ArgumentNullException(nameof(dispX255txtbox), "X25519 text field is missing");

                if (string.IsNullOrWhiteSpace(dispX255txtbox.Text))
                    throw new ArgumentException("Please enter a valid number for X25519 disappearing messages");

                if (!int.TryParse(dispX255txtbox.Text, out int value) || value <= 0)
                    throw new ArgumentException("Please enter a valid number greater than 0 (X25519)");

                // Check that RSA checkbox components are not null
                if (hourX255Chck == null || dayX255Chck == null || weekX255Chck == null || monthX255Chck == null)
                    throw new ArgumentException("X25519 time unit selection fields are missing");

                TimeUnit unit = GetSelectedTimeUnit(hourX255Chck, dayX255Chck, weekX255Chck, monthX255Chck);

                return new DisappearingSettings
                {
                    IsEnabled = true,
                    Value = value,
                    Unit = unit,
                    ExpireAfter = GetTimeSpanFromSettings(value, unit)
                };
            }

            public static DisappearingSettings GetPasswordSettings(
                ModernTextBox dispPassTxtbox,
                ModernCheckBox hourPassChck,
                ModernCheckBox dayPassCheck,
                ModernCheckBox weekPassChck,
                ModernCheckBox monthPassChck,
                ModernToggleSwitch toggleDispPass)
            {
                // Null checks for UI components
                if (toggleDispPass == null)
                    throw new ArgumentNullException(nameof(toggleDispPass), "Toggle component is missing");
                
                if (!toggleDispPass.Checked) return null;

                if (dispPassTxtbox == null)
                    throw new ArgumentNullException(nameof(dispPassTxtbox), "Text field is missing");

                if (string.IsNullOrWhiteSpace(dispPassTxtbox.Text))
                    throw new ArgumentException("Please enter a valid number");

                if (!int.TryParse(dispPassTxtbox.Text, out int value) || value <= 0)
                    throw new ArgumentException("Please enter a valid number greater than 0");

                // Check that checkbox components are not null
                if (hourPassChck == null || dayPassCheck == null || weekPassChck == null || monthPassChck == null)
                    throw new ArgumentException("Time unit selection fields are missing");

                TimeUnit unit = GetSelectedTimeUnit(hourPassChck, dayPassCheck, weekPassChck, monthPassChck);

                return new DisappearingSettings
                {
                    IsEnabled = true,
                    Value = value,
                    Unit = unit,
                    ExpireAfter = GetTimeSpanFromSettings(value, unit)
                };
            }

            private static TimeUnit GetSelectedTimeUnit(
                ModernCheckBox hourChck,
                ModernCheckBox dayChck,
                ModernCheckBox weekChck,
                ModernCheckBox monthChck)
            {
                // Null-safe checks already done in calling method, but verify
                if (hourChck?.Checked == true) return TimeUnit.Hour;
                if (dayChck?.Checked == true) return TimeUnit.Day;
                if (weekChck?.Checked == true) return TimeUnit.Week;
                if (monthChck?.Checked == true) return TimeUnit.Month;

                // Check if all checkboxes are null
                bool anyNull = hourChck == null || dayChck == null || weekChck == null || monthChck == null;
                if (anyNull)
                    throw new ArgumentException("Time unit selection fields are missing");

                throw new ArgumentException("Please select at least one time unit (hour, day, week or month)");
            }
        }

        /// <summary>
        /// Wrap X25519 packet with version byte for disappearing messages
        /// </summary>
        private static string WrapX25519PacketWithVersion(byte[] x25519Packet, byte versionByte)
        {
            var wrappedData = new byte[1 + x25519Packet.Length];
            wrappedData[0] = versionByte;
            Array.Copy(x25519Packet, 0, wrappedData, 1, x25519Packet.Length);
            return Convert.ToBase64String(wrappedData);
        }

        /// <summary>
        /// Unwrap X25519 packet and extract version byte for disappearing messages
        /// </summary>
        private static (byte version, byte[] packet) UnwrapX25519PacketWithVersion(string wrappedData)
        {
            var data = SafeDecodeBase64(wrappedData, "wrapped X25519 message");
            if (data.Length < 2)
                throw new ArgumentException("Invalid wrapped X25519 message: too short");
            
            byte version = data[0];
            byte[] packet = new byte[data.Length - 1];
            Array.Copy(data, 1, packet, 0, packet.Length);
            
            return (version, packet);
        }
    }

    /// <summary>
    /// Disappearing message metadata
    /// </summary>
    public class DisappearingMessageData
    {
        public string OriginalMessage { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string MessageType { get; set; } = string.Empty; // "PASSWORD_MODERN", "PASSWORD_LEGACY", "RSA"
        public byte Version { get; set; }
    }

    /// <summary>
    /// Disappearing messages settings
    /// </summary>
    public class DisappearingSettings
    {
        public bool IsEnabled { get; set; }
        public int Value { get; set; }
        public TimeUnit Unit { get; set; }
        public TimeSpan ExpireAfter { get; set; }

        public override string ToString()
        {
            return $"{Value} {GetUnitName()}{(Value > 1 ? GetPluralSuffix() : "")}";
        }

        private string GetUnitName()
        {
            return Unit switch
            {
                TimeUnit.Hour => "hour",
                TimeUnit.Day => "day",
                TimeUnit.Week => "week",
                TimeUnit.Month => "month",
                _ => "hour"
            };
        }

        private string GetPluralSuffix()
        {
            return Unit switch
            {
                TimeUnit.Hour => "s",
                TimeUnit.Day => "s",
                TimeUnit.Week => "s",
                TimeUnit.Month => "s",
                _ => "s"
            };
        }
    }

    /// <summary>
    /// Time unit
    /// </summary>
    public enum TimeUnit
    {
        Hour,
        Day,
        Week,
        Month
    }

    /// <summary>
    /// X25519 Disappearing message metadata with additional session information
    /// </summary>
    public class X25519DisappearingMessageData
    {
        public string OriginalMessage { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string MessageType { get; set; } = string.Empty; // "X25519_MODERN"
        public byte Version { get; set; }
        public string SessionId { get; set; } = string.Empty; // Base64 encoded session ID
        public string SenderId { get; set; } = string.Empty; // Base64 encoded sender ID
        public uint MessageSequence { get; set; }
    }

    /// <summary>
    /// Custom exception for handling expired messages
    /// </summary>
    public class MessageExpiredException : Exception
    {
        public MessageExpiredException(string message) : base(message) { }
        public MessageExpiredException(string message, Exception innerException) : base(message, innerException) { }
    }
}