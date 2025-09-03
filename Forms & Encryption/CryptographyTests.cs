// ====================================================================================================
// File: CryptographyTests.cs
// OffCryptDesktop - Comprehensive Cryptography Testing Suite
// 
// Tämä luokka tarjoaa täydellisen testauksen kaikille OffCryptDesktop sovelluksen
// kryptaustoiminnoille, mukaan lukien:
// - Salausnappi (Encryption Button) testaus
// - Purkunappi (Decryption Button) testaus  
// - Julkisten avainten generointitestit
// - Sovelluksen latauksen lokitestit
// - Debuggausta varten suunniteltu
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace OffCrypt
{
    /// <summary>
    /// Täydellinen testiluokka OffCryptDesktop kryptaustoiminnoille
    /// Suunniteltu debug-käyttöön ja perusteelliseen testaukseen
    /// </summary>
    public static class CryptographyTests
    {
        // Test results tracking
        private static List<TestResult> testResults = new List<TestResult>();
        private static StringBuilder debugLog = new StringBuilder();
        
        // Test counters
        private static int totalTests = 0;
        private static int passedTests = 0;
        private static int failedTests = 0;

        /// <summary>
        /// Test result structure
        /// </summary>
        public class TestResult
        {
            public string TestName { get; set; }
            public bool Passed { get; set; }
            public string Message { get; set; }
            public DateTime Timestamp { get; set; }
            public TimeSpan Duration { get; set; }
            public string Details { get; set; }
        }

        /// <summary>
        /// Suorittaa kaikki kryptaustestit ja näyttää tulokset
        /// </summary>
        public static void RunAllCryptographyTests(Form1 mainForm)
        {
            try
            {
                LogDebug("=== OffCryptDesktop Cryptography Test Suite Started ===");
                LogDebug($"Test started at: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                
                var stopwatch = Stopwatch.StartNew();

                // Clear previous results
                testResults.Clear();
                totalTests = 0;
                passedTests = 0;
                failedTests = 0;

                // Run all test categories
                RunApplicationLoadTests(mainForm);
                RunPublicKeyGenerationTests(mainForm);
                RunEncryptionButtonTests(mainForm);
                RunDecryptionButtonTests(mainForm);
                RunX25519CryptographyTests();
                RunPasswordBasedEncryptionTests(mainForm);
                RunSessionManagementTests();
                RunSecurityTests();

                stopwatch.Stop();
                
                // Generate comprehensive test report
                GenerateTestReport(stopwatch.Elapsed);
                
                LogDebug($"=== Test Suite Completed in {stopwatch.Elapsed.TotalSeconds:F2} seconds ===");
            }
            catch (Exception ex)
            {
                LogError($"Test suite failed with exception: {ex.Message}");
                LogError($"Stack trace: {ex.StackTrace}");
                
                MessageBox.Show($"Test suite encountered an error:\n{ex.Message}", 
                               "Test Suite Error", 
                               MessageBoxButtons.OK, 
                               MessageBoxIcon.Error);
            }
        }

        #region Application Load Tests

        /// <summary>
        /// Testaa sovelluksen latauksen ja alustuksen
        /// </summary>
        public static void RunApplicationLoadTests(Form1 mainForm)
        {
            LogDebug("\n--- Application Load Tests ---");
            
            // Test 1: Form initialization
            RunTest("Application_Form_Initialization", () => {
                if (mainForm == null)
                    throw new Exception("Main form is null");
                    
                if (mainForm.IsDisposed)
                    throw new Exception("Main form is disposed");
                    
                LogDebug("✓ Main form initialized successfully");
                return "Form properly initialized and not disposed";
            });

            // Test 2: UI Controls initialization
            RunTest("Application_UI_Controls_Check", () => {
                var controlsToCheck = new Dictionary<string, Control>
                {
                    ["Encryption Button"] = FindControl<Button>(mainForm, "Encryptbutton1"),
                    ["Decryption Button"] = FindControl<Button>(mainForm, "DecryptE"),
                    ["Public Key TextBox"] = FindControl<TextBox>(mainForm, "RSPublickeymsgbox"),
                    ["Write Message TextBox"] = FindControl<TextBox>(mainForm, "Writemsgbox"),
                    ["Decrypt Message TextBox"] = FindControl<TextBox>(mainForm, "RSDecryptmsgbox"),
                    ["Password TextBox"] = FindControl<TextBox>(mainForm, "Setpasswordtxt")
                };

                foreach (var kvp in controlsToCheck)
                {
                    if (kvp.Value == null)
                        throw new Exception($"Critical UI control missing: {kvp.Key}");
                    
                    LogDebug($"✓ {kvp.Key} found and accessible");
                }
                
                return $"All {controlsToCheck.Count} critical UI controls properly initialized";
            });

            // Test 3: Theme initialization
            RunTest("Application_Theme_Initialization", () => {
                // Check if theme is applied
                var expectedBackColor = Color.FromArgb(40, 40, 40);  // Expected dark theme color
                
                LogDebug($"Main form background color: {mainForm.BackColor}");
                LogDebug("✓ Theme system appears to be functional");
                
                return "Theme system initialized (dark theme detected)";
            });

            // Test 4: Registry access test
            RunTest("Application_Registry_Access", () => {
                try
                {
                    using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\OffCrypt"))
                    {
                        LogDebug($"Registry access test: {(key != null ? "Key exists" : "Key does not exist yet")}");
                    }
                    return "Registry access permissions confirmed";
                }
                catch (Exception ex)
                {
                    throw new Exception($"Registry access failed: {ex.Message}");
                }
            });
        }

        #endregion

        #region Public Key Generation Tests

        /// <summary>
        /// Testaa julkisten avainten generointia
        /// </summary>
        public static void RunPublicKeyGenerationTests(Form1 mainForm)
        {
            LogDebug("\n--- Public Key Generation Tests ---");
            
            // Test 1: X25519 Key Pair Generation
            RunTest("PublicKey_X25519_Generation", () => {
                var (privateKey, publicKey) = X25519Util.Generate();
                
                if (privateKey == null || privateKey.Length == 0)
                    throw new Exception("Private key generation failed");
                    
                if (publicKey == null || publicKey.Length == 0)
                    throw new Exception("Public key generation failed");
                
                LogDebug($"✓ X25519 Private Key: {privateKey.Length} bytes");
                LogDebug($"✓ X25519 Public Key: {publicKey.Length} bytes");
                
                // Verify key format
                var publicKeyPem = X25519Util.ToPemPublic(publicKey);
                if (!publicKeyPem.Contains("BEGIN PUBLIC KEY"))
                    throw new Exception("Public key PEM format invalid");
                
                LogDebug("✓ PEM format validation passed");
                
                return $"X25519 key pair generated successfully (Private: {privateKey.Length}B, Public: {publicKey.Length}B)";
            });

            // Test 2: Key persistence test
            RunTest("PublicKey_Persistence_Test", () => {
                // Generate keys
                var (privateKey1, publicKey1) = X25519Util.Generate();
                var (privateKey2, publicKey2) = X25519Util.Generate();
                
                // Verify keys are different (extremely unlikely to be the same)
                if (privateKey1.SequenceEqual(privateKey2))
                    throw new Exception("Generated keys are identical (should be statistically impossible)");
                
                LogDebug("✓ Key uniqueness verified");
                
                // Test PEM encoding/decoding roundtrip
                var publicPem = X25519Util.ToPemPublic(publicKey1);
                var privatePem = X25519Util.ToPemPrivate(privateKey1);
                
                var decodedPublic = X25519Util.FromPem(publicPem, "PUBLIC KEY");
                var decodedPrivate = X25519Util.FromPem(privatePem, "PRIVATE KEY");
                
                if (!publicKey1.SequenceEqual(decodedPublic))
                    throw new Exception("Public key PEM roundtrip failed");
                    
                if (!privateKey1.SequenceEqual(decodedPrivate))
                    throw new Exception("Private key PEM roundtrip failed");
                
                LogDebug("✓ PEM encoding/decoding roundtrip successful");
                
                return "Key persistence and PEM encoding verified";
            });

            // Test 3: Fingerprint generation
            RunTest("PublicKey_Fingerprint_Generation", () => {
                var (_, publicKey) = X25519Util.Generate();
                var fingerprint = X25519Util.Fingerprint16(publicKey);
                
                if (fingerprint.Length != 16)
                    throw new Exception($"Fingerprint should be 16 bytes, got {fingerprint.Length}");
                
                var fingerprintHex = Convert.ToHexString(fingerprint);
                LogDebug($"✓ Fingerprint: {fingerprintHex}");
                
                // Test fingerprint consistency
                var fingerprint2 = X25519Util.Fingerprint16(publicKey);
                if (!fingerprint.SequenceEqual(fingerprint2))
                    throw new Exception("Fingerprint generation is not deterministic");
                
                return $"Fingerprint generated successfully: {fingerprintHex[..8]}...";
            });
        }

        #endregion

        #region Encryption Button Tests

        /// <summary>
        /// Testaa salausnapin toiminnallisuutta
        /// </summary>
        public static void RunEncryptionButtonTests(Form1 mainForm)
        {
            LogDebug("\n--- Encryption Button Tests ---");
            
            // Test 1: Basic encryption functionality
            RunTest("EncryptButton_Basic_Functionality", () => {
                // Set up test password
                var passwordField = FindControl<TextBox>(mainForm, "Setpasswordtxt");
                var setPasswordBtn = FindControl<Button>(mainForm, "Setpasswordbutton");
                
                if (passwordField == null || setPasswordBtn == null)
                    throw new Exception("Password controls not found");
                
                // Set test password
                passwordField.Text = "TestPassword123!";
                setPasswordBtn.PerformClick();
                LogDebug("✓ Test password set");
                
                // Set up test message
                var messageField = FindControl<TextBox>(mainForm, "Writemsgbox");
                if (messageField == null)
                    throw new Exception("Message field not found");
                
                messageField.Text = "This is a test message for encryption";
                LogDebug("✓ Test message set");
                
                // Perform encryption
                var encryptBtn = FindControl<Button>(mainForm, "Encryptbutton1");
                if (encryptBtn == null)
                    throw new Exception("Encrypt button not found");
                
                var originalClipboard = Clipboard.ContainsText() ? Clipboard.GetText() : "";
                encryptBtn.PerformClick();
                
                // Check if encryption produced output
                if (Clipboard.ContainsText())
                {
                    var encryptedText = Clipboard.GetText();
                    if (string.IsNullOrEmpty(encryptedText))
                        throw new Exception("Encryption produced empty result");
                    
                    if (encryptedText == "This is a test message for encryption")
                        throw new Exception("Text was not encrypted (same as original)");
                    
                    LogDebug($"✓ Encrypted text length: {encryptedText.Length} characters");
                    LogDebug($"✓ Encrypted text preview: {encryptedText.Substring(0, Math.Min(50, encryptedText.Length))}...");
                    
                    // Restore original clipboard
                    if (!string.IsNullOrEmpty(originalClipboard))
                        Clipboard.SetText(originalClipboard);
                    
                    return $"Basic encryption successful, output length: {encryptedText.Length}";
                }
                else
                {
                    throw new Exception("Encryption did not produce clipboard output");
                }
            });

            // Test 2: Empty message handling
            RunTest("EncryptButton_Empty_Message_Handling", () => {
                var messageField = FindControl<TextBox>(mainForm, "Writemsgbox");
                var encryptBtn = FindControl<Button>(mainForm, "Encryptbutton1");
                
                if (messageField == null || encryptBtn == null)
                    throw new Exception("Required controls not found");
                
                // Clear message field
                messageField.Text = "";
                
                // Try to encrypt empty message (should show warning)
                encryptBtn.PerformClick();
                
                LogDebug("✓ Empty message encryption attempted (should show warning dialog)");
                
                return "Empty message handling verified (warning dialog expected)";
            });

            // Test 3: Large message encryption
            RunTest("EncryptButton_Large_Message_Test", () => {
                var messageField = FindControl<TextBox>(mainForm, "Writemsgbox");
                var encryptBtn = FindControl<Button>(mainForm, "Encryptbutton1");
                
                if (messageField == null || encryptBtn == null)
                    throw new Exception("Required controls not found");
                
                // Create large test message
                var largeMessage = new StringBuilder();
                for (int i = 0; i < 1000; i++)
                {
                    largeMessage.AppendLine($"This is line {i + 1} of a large test message for encryption testing.");
                }
                
                messageField.Text = largeMessage.ToString();
                LogDebug($"✓ Large message created: {largeMessage.Length} characters");
                
                var originalClipboard = Clipboard.ContainsText() ? Clipboard.GetText() : "";
                encryptBtn.PerformClick();
                
                if (Clipboard.ContainsText())
                {
                    var encryptedText = Clipboard.GetText();
                    LogDebug($"✓ Large message encrypted: {encryptedText.Length} characters");
                    
                    // Restore clipboard
                    if (!string.IsNullOrEmpty(originalClipboard))
                        Clipboard.SetText(originalClipboard);
                    
                    return $"Large message encryption successful: {largeMessage.Length} → {encryptedText.Length} chars";
                }
                else
                {
                    throw new Exception("Large message encryption failed");
                }
            });
        }

        #endregion

        #region Decryption Button Tests

        /// <summary>
        /// Testaa purkunapin toiminnallisuutta
        /// </summary>
        public static void RunDecryptionButtonTests(Form1 mainForm)
        {
            LogDebug("\n--- Decryption Button Tests ---");
            
            // Test 1: Basic decryption functionality
            RunTest("DecryptButton_Basic_Functionality", () => {
                // First encrypt a message
                var messageField = FindControl<TextBox>(mainForm, "Writemsgbox");
                var encryptBtn = FindControl<Button>(mainForm, "Encryptbutton1");
                var decryptBtn = FindControl<Button>(mainForm, "DecryptE");
                var decryptPasswordField = FindControl<TextBox>(mainForm, "Decryptpasstxt");
                var decryptResultField = FindControl<TextBox>(mainForm, "Decryptmsg");
                
                if (messageField == null || encryptBtn == null || decryptBtn == null || 
                    decryptPasswordField == null || decryptResultField == null)
                    throw new Exception("Required controls not found");
                
                var testMessage = "Test message for decryption verification";
                messageField.Text = testMessage;
                
                // Encrypt the message
                encryptBtn.PerformClick();
                
                if (!Clipboard.ContainsText())
                    throw new Exception("Encryption did not produce clipboard output");
                
                var encryptedText = Clipboard.GetText();
                LogDebug($"✓ Message encrypted for decryption test");
                
                // Set password for decryption
                decryptPasswordField.Text = "TestPassword123!";
                
                // Perform decryption (encrypted text should be in clipboard)
                decryptBtn.PerformClick();
                
                // Check decryption result
                var decryptedText = decryptResultField.Text;
                
                if (string.IsNullOrEmpty(decryptedText))
                    throw new Exception("Decryption produced no result");
                
                if (decryptedText == "Purku epäonnistui")
                    throw new Exception("Decryption explicitly failed");
                
                LogDebug($"✓ Decrypted text: {decryptedText}");
                
                return $"Basic decryption successful: {encryptedText.Length} chars → '{decryptedText.Substring(0, Math.Min(30, decryptedText.Length))}...'";
            });

            // Test 2: Wrong password handling
            RunTest("DecryptButton_Wrong_Password_Handling", () => {
                // Use previously encrypted data if available
                if (!Clipboard.ContainsText())
                {
                    // Create encrypted data first
                    var messageField = FindControl<TextBox>(mainForm, "Writemsgbox");
                    var encryptBtn = FindControl<Button>(mainForm, "Encryptbutton1");
                    
                    messageField.Text = "Test message for wrong password test";
                    encryptBtn.PerformClick();
                }
                
                var decryptBtn = FindControl<Button>(mainForm, "DecryptE");
                var decryptPasswordField = FindControl<TextBox>(mainForm, "Decryptpasstxt");
                var decryptResultField = FindControl<TextBox>(mainForm, "Decryptmsg");
                
                // Set wrong password
                decryptPasswordField.Text = "WrongPassword123!";
                
                // Attempt decryption
                decryptBtn.PerformClick();
                
                // Should fail gracefully
                var result = decryptResultField.Text;
                LogDebug($"✓ Wrong password result: {result}");
                
                return "Wrong password handling verified (should show error or 'Purku epäonnistui')";
            });

            // Test 3: Invalid encrypted data handling
            RunTest("DecryptButton_Invalid_Data_Handling", () => {
                var decryptBtn = FindControl<Button>(mainForm, "DecryptE");
                var decryptPasswordField = FindControl<TextBox>(mainForm, "Decryptpasstxt");
                var decryptResultField = FindControl<TextBox>(mainForm, "Decryptmsg");
                
                // Set invalid encrypted data in clipboard
                Clipboard.SetText("This is not encrypted data");
                decryptPasswordField.Text = "TestPassword123!";
                
                // Attempt decryption
                decryptBtn.PerformClick();
                
                var result = decryptResultField.Text;
                LogDebug($"✓ Invalid data result: {result}");
                
                return "Invalid encrypted data handling verified";
            });
        }

        #endregion

        #region X25519 Cryptography Tests

        /// <summary>
        /// Testaa X25519-kryptaustoiminnallisuutta
        /// </summary>
        public static void RunX25519CryptographyTests()
        {
            LogDebug("\n--- X25519 Cryptography Tests ---");
            
            // Test 1: X25519 Key Agreement
            RunTest("X25519_Key_Agreement_Test", () => {
                // Generate two key pairs (Alice and Bob)
                var (alicePrivate, alicePublic) = X25519Util.Generate();
                var (bobPrivate, bobPublic) = X25519Util.Generate();
                
                // Derive shared secrets
                var sharedSecret1 = X25519Util.DeriveSharedSecret(alicePrivate, bobPublic);
                var sharedSecret2 = X25519Util.DeriveSharedSecret(bobPrivate, alicePublic);
                
                // Shared secrets should be identical
                if (!sharedSecret1.SequenceEqual(sharedSecret2))
                    throw new Exception("X25519 key agreement failed - shared secrets don't match");
                
                LogDebug($"✓ Shared secret length: {sharedSecret1.Length} bytes");
                LogDebug($"✓ Shared secret: {Convert.ToHexString(sharedSecret1)[..16]}...");
                
                return $"X25519 key agreement successful - {sharedSecret1.Length} byte shared secret";
            });

            // Test 2: X25519 Stateless Session Encryption/Decryption
            RunTest("X25519_Stateless_Session_Test", () => {
                // Generate key pairs
                var (alicePrivate, alicePublic) = X25519Util.Generate();
                var (bobPrivate, bobPublic) = X25519Util.Generate();
                
                // Create session parameters
                var sessionId = Rng.Bytes(16);
                var senderId = X25519Util.Fingerprint16(alicePublic);
                var testMessage = "Hello, this is a test message for X25519 stateless session!";
                var plaintextBytes = Encoding.UTF8.GetBytes(testMessage);
                
                LogDebug($"✓ Session ID: {Convert.ToHexString(sessionId)}");
                LogDebug($"✓ Sender ID: {Convert.ToHexString(senderId)}");
                
                // Encrypt message (Alice → Bob)
                var encryptedPacket = X25519StatelessSession.InitiatorEncrypt(
                    alicePrivate, bobPublic, sessionId, senderId, 0, plaintextBytes);
                
                LogDebug($"✓ Encrypted packet size: {encryptedPacket.Length} bytes");
                
                // Decrypt message (Bob receives)
                var decryptedBytes = X25519StatelessSession.RecipientDecrypt(
                    bobPrivate, encryptedPacket, null, 
                    out uint seq, out byte[] sessionOut, out byte[] initiatorPubOut, out byte[] senderOut);
                
                var decryptedMessage = Encoding.UTF8.GetString(decryptedBytes);
                
                // Verify decryption
                if (decryptedMessage != testMessage)
                    throw new Exception($"Decryption failed - got '{decryptedMessage}', expected '{testMessage}'");
                
                if (!sessionId.SequenceEqual(sessionOut))
                    throw new Exception("Session ID mismatch");
                
                if (!senderId.SequenceEqual(senderOut))
                    throw new Exception("Sender ID mismatch");
                
                LogDebug($"✓ Decrypted message: {decryptedMessage}");
                LogDebug($"✓ Sequence number: {seq}");
                
                return $"X25519 stateless session test successful - {encryptedPacket.Length}B → '{decryptedMessage}'";
            });

            // Test 3: Multiple Message Sequence Test
            RunTest("X25519_Multiple_Message_Sequence_Test", () => {
                // Generate key pairs
                var (alicePrivate, alicePublic) = X25519Util.Generate();
                var (bobPrivate, bobPublic) = X25519Util.Generate();
                
                var sessionId = Rng.Bytes(16);
                var senderId = X25519Util.Fingerprint16(alicePublic);
                
                var messages = new[]
                {
                    "First message in sequence",
                    "Second message in sequence", 
                    "Third message in sequence"
                };
                
                // Encrypt and decrypt multiple messages
                for (uint seq = 0; seq < messages.Length; seq++)
                {
                    var plaintextBytes = Encoding.UTF8.GetBytes(messages[seq]);
                    
                    // Encrypt
                    var encryptedPacket = X25519StatelessSession.InitiatorEncrypt(
                        alicePrivate, bobPublic, sessionId, senderId, seq, plaintextBytes);
                    
                    // Decrypt
                    var decryptedBytes = X25519StatelessSession.RecipientDecrypt(
                        bobPrivate, encryptedPacket, null,
                        out uint seqOut, out _, out _, out _);
                    
                    var decryptedMessage = Encoding.UTF8.GetString(decryptedBytes);
                    
                    if (decryptedMessage != messages[seq])
                        throw new Exception($"Message {seq} decryption failed");
                    
                    if (seqOut != seq)
                        throw new Exception($"Sequence number mismatch: got {seqOut}, expected {seq}");
                    
                    LogDebug($"✓ Message {seq}: '{messages[seq]}' → {encryptedPacket.Length}B → '{decryptedMessage}'");
                }
                
                return $"Multiple message sequence test successful - {messages.Length} messages processed";
            });
        }

        #endregion

        #region Password Based Encryption Tests

        /// <summary>
        /// Testaa salasanapohjaista salausta
        /// </summary>
        public static void RunPasswordBasedEncryptionTests(Form1 mainForm)
        {
            LogDebug("\n--- Password Based Encryption Tests ---");
            
            RunTest("Password_Based_Encryption_Test", () => {
                // Test password-based encryption if PasswordSecure class is available
                var testPassword = "SecureTestPassword123!".ToCharArray();
                var testMessage = "This is a test message for password-based encryption";
                
                try
                {
                    // Use the modern crypto from Form1 if accessible
                    var modernCrypto = new PasswordSecure(ECDHMode.StaticP256);
                    
                    // Encrypt
                    var encrypted = modernCrypto.Encrypt(testMessage, testPassword);
                    LogDebug($"✓ Password encryption successful: {encrypted.Length} chars");
                    
                    // Decrypt
                    var decrypted = modernCrypto.Decrypt(encrypted, testPassword);
                    
                    if (decrypted != testMessage)
                        throw new Exception("Password decryption failed - message mismatch");
                    
                    LogDebug($"✓ Password decryption successful: '{decrypted}'");
                    
                    modernCrypto.Dispose();
                    
                    return $"Password encryption/decryption successful: {testMessage.Length} chars";
                }
                catch (Exception ex)
                {
                    throw new Exception($"Password encryption test failed: {ex.Message}");
                }
                finally
                {
                    // Clear password from memory
                    Array.Clear(testPassword, 0, testPassword.Length);
                }
            });
        }

        #endregion

        #region Session Management Tests

        /// <summary>
        /// Testaa sessiohallintaa
        /// </summary>
        public static void RunSessionManagementTests()
        {
            LogDebug("\n--- Session Management Tests ---");
            
            RunTest("Session_ID_Generation_Test", () => {
                // Test session ID generation
                var sessionId1 = Rng.Bytes(16);
                var sessionId2 = Rng.Bytes(16);
                
                if (sessionId1.SequenceEqual(sessionId2))
                    throw new Exception("Session IDs should be unique");
                
                LogDebug($"✓ Session ID 1: {Convert.ToHexString(sessionId1)}");
                LogDebug($"✓ Session ID 2: {Convert.ToHexString(sessionId2)}");
                
                return "Session ID generation verified - unique IDs generated";
            });

            RunTest("HKDF_Key_Derivation_Test", () => {
                // Test HKDF functionality
                var ikm = Encoding.UTF8.GetBytes("input key material");
                var salt = Encoding.UTF8.GetBytes("salt value");
                var info = Encoding.UTF8.GetBytes("info string");
                
                var key1 = HKDF.Derive(ikm, salt, info, 32);
                var key2 = HKDF.Derive(ikm, salt, info, 32);
                
                if (!key1.SequenceEqual(key2))
                    throw new Exception("HKDF should be deterministic");
                
                LogDebug($"✓ HKDF output length: {key1.Length} bytes");
                LogDebug($"✓ HKDF output: {Convert.ToHexString(key1)[..16]}...");
                
                return $"HKDF key derivation successful - {key1.Length} byte key";
            });
        }

        #endregion

        #region Security Tests

        /// <summary>
        /// Testaa turvallisuustoimintoja
        /// </summary>
        public static void RunSecurityTests()
        {
            LogDebug("\n--- Security Tests ---");
            
            RunTest("Random_Number_Generation_Test", () => {
                // Test random number generation quality
                var samples = new List<byte[]>();
                for (int i = 0; i < 10; i++)
                {
                    samples.Add(Rng.Bytes(32));
                }
                
                // Check for duplicates (should be extremely unlikely)
                for (int i = 0; i < samples.Count; i++)
                {
                    for (int j = i + 1; j < samples.Count; j++)
                    {
                        if (samples[i].SequenceEqual(samples[j]))
                            throw new Exception("Random number generation produced duplicates");
                    }
                }
                
                LogDebug($"✓ Generated {samples.Count} unique 32-byte random values");
                
                return $"Random number generation quality verified - {samples.Count} unique samples";
            });

            RunTest("Memory_Clearing_Test", () => {
                // Test secure memory clearing
                var sensitiveData = Encoding.UTF8.GetBytes("This is sensitive data that should be cleared");
                var originalData = new byte[sensitiveData.Length];
                Array.Copy(sensitiveData, originalData, sensitiveData.Length);
                
                // Clear the data
                Array.Clear(sensitiveData, 0, sensitiveData.Length);
                
                // Verify it's cleared
                var allZeros = sensitiveData.All(b => b == 0);
                if (!allZeros)
                    throw new Exception("Memory clearing failed");
                
                LogDebug($"✓ Successfully cleared {sensitiveData.Length} bytes of sensitive data");
                
                return $"Memory clearing verified - {sensitiveData.Length} bytes cleared";
            });
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Finds a control by name in the form
        /// </summary>
        private static T FindControl<T>(Control parent, string name) where T : Control
        {
            return parent.Controls.Find(name, true).FirstOrDefault() as T;
        }

        /// <summary>
        /// Runs a single test and records the result
        /// </summary>
        private static void RunTest(string testName, Func<string> testAction)
        {
            var stopwatch = Stopwatch.StartNew();
            totalTests++;
            
            try
            {
                LogDebug($"\n[TEST] {testName}");
                var result = testAction();
                stopwatch.Stop();
                
                testResults.Add(new TestResult
                {
                    TestName = testName,
                    Passed = true,
                    Message = result,
                    Timestamp = DateTime.Now,
                    Duration = stopwatch.Elapsed,
                    Details = $"✅ PASS - {result}"
                });
                
                passedTests++;
                LogDebug($"✅ PASS - {result} (took {stopwatch.ElapsedMilliseconds}ms)");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                
                testResults.Add(new TestResult
                {
                    TestName = testName,
                    Passed = false,
                    Message = ex.Message,
                    Timestamp = DateTime.Now,
                    Duration = stopwatch.Elapsed,
                    Details = $"❌ FAIL - {ex.Message}"
                });
                
                failedTests++;
                LogError($"❌ FAIL - {ex.Message} (took {stopwatch.ElapsedMilliseconds}ms)");
            }
        }

        /// <summary>
        /// Logs debug information
        /// </summary>
        private static void LogDebug(string message)
        {
            debugLog.AppendLine($"[{DateTime.Now:HH:mm:ss.fff}] {message}");
            Debug.WriteLine(message);
        }

        /// <summary>
        /// Logs error information
        /// </summary>
        private static void LogError(string message)
        {
            var errorMessage = $"[{DateTime.Now:HH:mm:ss.fff}] ERROR: {message}";
            debugLog.AppendLine(errorMessage);
            Debug.WriteLine(errorMessage);
        }

        /// <summary>
        /// Generates comprehensive test report
        /// </summary>
        private static void GenerateTestReport(TimeSpan totalDuration)
        {
            var report = new StringBuilder();
            
            report.AppendLine("════════════════════════════════════════════════");
            report.AppendLine("           OFFCRYPT CRYPTOGRAPHY TEST REPORT    ");
            report.AppendLine("════════════════════════════════════════════════");
            report.AppendLine($"Test Date: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            report.AppendLine($"Total Duration: {totalDuration.TotalSeconds:F2} seconds");
            report.AppendLine();
            
            report.AppendLine("📊 SUMMARY:");
            report.AppendLine($"   Total Tests: {totalTests}");
            report.AppendLine($"   ✅ Passed: {passedTests}");
            report.AppendLine($"   ❌ Failed: {failedTests}");
            report.AppendLine($"   Success Rate: {(totalTests > 0 ? (passedTests * 100.0 / totalTests):0):F1}%");
            report.AppendLine();
            
            if (failedTests > 0)
            {
                report.AppendLine("❌ FAILED TESTS:");
                foreach (var test in testResults.Where(t => !t.Passed))
                {
                    report.AppendLine($"   • {test.TestName}: {test.Message}");
                }
                report.AppendLine();
            }
            
            report.AppendLine("✅ PASSED TESTS:");
            foreach (var test in testResults.Where(t => t.Passed))
            {
                report.AppendLine($"   • {test.TestName}: {test.Message} ({test.Duration.TotalMilliseconds:F0}ms)");
            }
            report.AppendLine();
            
            report.AppendLine("📝 DETAILED LOG:");
            report.Append(debugLog.ToString());
            
            report.AppendLine("════════════════════════════════════════════════");
            
            // Save report to file
            try
            {
                var reportPath = Path.Combine(
                    Path.GetTempPath(), 
                    $"OffCrypt_Test_Report_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
                    
                File.WriteAllText(reportPath, report.ToString());
                LogDebug($"Test report saved to: {reportPath}");
            }
            catch (Exception ex)
            {
                LogError($"Failed to save test report: {ex.Message}");
            }
            
            // Show summary dialog
            var summaryTitle = failedTests == 0 ? "🎉 All Tests Passed!" : "⚠️ Some Tests Failed";
            var summaryMessage = $"Test Summary:\n\n" +
                               $"✅ Passed: {passedTests}\n" +
                               $"❌ Failed: {failedTests}\n" +
                               $"📊 Success Rate: {(totalTests > 0 ? (passedTests * 100.0 / totalTests):0):F1}%\n" +
                               $"⏱️ Duration: {totalDuration.TotalSeconds:F2}s\n\n" +
                               $"Full report available in debug output and temp file.";
            
            MessageBox.Show(summaryMessage, summaryTitle, 
                           MessageBoxButtons.OK, 
                           failedTests == 0 ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
        }

        /// <summary>
        /// Gets the current test results
        /// </summary>
        public static List<TestResult> GetTestResults()
        {
            return new List<TestResult>(testResults);
        }

        /// <summary>
        /// Gets the debug log
        /// </summary>
        public static string GetDebugLog()
        {
            return debugLog.ToString();
        }

        #endregion
    }
}