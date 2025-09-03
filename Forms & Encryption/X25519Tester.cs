using System;
using System.Text;
using System.Windows.Forms;

namespace OffCrypt
{
    /// <summary>
    /// X25519Tester - Comprehensive test class for testing X25519 cryptography
    /// Tests all core functions: key pair generation, encryption, decryption, PEM conversions
    /// </summary>
    public static class X25519Tester
    {
        /// <summary>
        /// Run all X25519 tests and display results
        /// </summary>
        public static void RunAllTests()
        {
            var results = new StringBuilder();
            results.AppendLine("🧪 X25519 CRYPTOGRAPHY TESTS");
            results.AppendLine("========================");
            results.AppendLine();

            int passed = 0;
            int total = 0;

            // Test 1: Key pair generation
            total++;
            if (TestKeyGeneration(results))
                passed++;

            // Test 2: PEM conversions
            total++;
            if (TestPemConversions(results))
                passed++;

            // Test 3: Basic Encrypt/Decrypt
            total++;
            if (TestBasicEncryptDecrypt(results))
                passed++;

            // Test 4: Cross-key communication (Alice -> Bob)
            total++;
            if (TestCrossKeyEncryption(results))
                passed++;

            // Test 5: Session management
            total++;
            if (TestSessionManagement(results))
                passed++;

            // Test 6: Error situations
            total++;
            if (TestErrorHandling(results))
                passed++;

            // Test 7: Performance test
            total++;
            if (TestPerformance(results))
                passed++;

            // Summary
            results.AppendLine();
            results.AppendLine("=== TEST SUMMARY ===");
            results.AppendLine($"Passed tests: {passed}/{total}");
            results.AppendLine($"Success percentage: {(passed * 100.0 / total):F1}%");
            
            if (passed == total)
            {
                results.AppendLine("✅ ALL TESTS PASSED!");
                results.AppendLine("X25519 cryptography works flawlessly.");
            }
            else
            {
                results.AppendLine("❌ SOME TESTS FAILED!");
                results.AppendLine("Check error messages above.");
            }

            // Show results
            using var resultForm = new Form
            {
                Text = "X25519 Test Results",
                Size = new System.Drawing.Size(800, 600),
                StartPosition = FormStartPosition.CenterScreen
            };

            var textBox = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Both,
                Dock = DockStyle.Fill,
                Font = new System.Drawing.Font("Consolas", 9),
                ReadOnly = true,
                Text = results.ToString()
            };

            resultForm.Controls.Add(textBox);
            resultForm.ShowDialog();
        }

        /// <summary>
        /// Test key pair generation
        /// </summary>
        private static bool TestKeyGeneration(StringBuilder results)
        {
            results.AppendLine("🔑 Test 1: Key pair generation");
            try
            {
                // Generate key pair
                var (privateKey, publicKey) = X25519Util.Generate();

                if (privateKey == null || privateKey.Length == 0)
                {
                    results.AppendLine("❌ Private key is empty");
                    return false;
                }

                if (publicKey == null || publicKey.Length == 0)
                {
                    results.AppendLine("❌ Public key is empty");
                    return false;
                }

                // Check key lengths
                results.AppendLine($"   📏 Private key: {privateKey.Length} bytes");
                results.AppendLine($"   📏 Public key: {publicKey.Length} bytes");

                // Test that public key can be derived from private
                var derivedPublic = X25519Util.PublicFromPrivate(privateKey);
                if (!ArraysEqual(publicKey, derivedPublic))
                {
                    results.AppendLine("❌ Derived public key doesn't match original");
                    return false;
                }

                results.AppendLine("✅ Key pair generation successful");
                results.AppendLine();
                return true;
            }
            catch (Exception ex)
            {
                results.AppendLine($"❌ Error: {ex.Message}");
                results.AppendLine();
                return false;
            }
        }

        /// <summary>
        /// Testaa PEM-muunnoksia
        /// </summary>
        private static bool TestPemConversions(StringBuilder results)
        {
            results.AppendLine("📄 Testi 2: PEM-muunnokset");
            try
            {
                var (privateKey, publicKey) = X25519Util.Generate();

                // Muunna PEM-muotoon
                string publicPem = X25519Util.ToPemPublic(publicKey);
                string privatePem = X25519Util.ToPemPrivate(privateKey);

                if (string.IsNullOrEmpty(publicPem) || !publicPem.Contains("BEGIN PUBLIC KEY"))
                {
                    results.AppendLine("❌ Julkisen avaimen PEM-muunnos epäonnistui");
                    return false;
                }

                if (string.IsNullOrEmpty(privatePem) || !privatePem.Contains("BEGIN PRIVATE KEY"))
                {
                    results.AppendLine("❌ Yksityisen avaimen PEM-muunnos epäonnistui");
                    return false;
                }

                // Convert back
                var publicRestored = X25519Util.FromPem(publicPem, "PUBLIC KEY");
                var privateRestored = X25519Util.FromPem(privatePem, "PRIVATE KEY");

                if (!ArraysEqual(publicKey, publicRestored))
                {
                    results.AppendLine("❌ Public key doesn't restore from PEM correctly");
                    return false;
                }

                if (!ArraysEqual(privateKey, privateRestored))
                {
                    results.AppendLine("❌ Private key doesn't restore from PEM correctly");
                    return false;
                }

                results.AppendLine("   📝 PEM public key:");
                results.AppendLine($"   {publicPem.Split('\n')[0]}...");
                results.AppendLine("✅ PEM conversions succeeded");
                results.AppendLine();
                return true;
            }
            catch (Exception ex)
            {
                results.AppendLine($"❌ Virhe: {ex.Message}");
                results.AppendLine();
                return false;
            }
        }

        /// <summary>
        /// Test basic encrypt/decrypt operations
        /// </summary>
        private static bool TestBasicEncryptDecrypt(StringBuilder results)
        {
            results.AppendLine("🔐 Test 3: Basic Encrypt/Decrypt");
            try
            {
                // Create key pair
                var (privateKey, publicKey) = X25519Util.Generate();
                
                // Test data
                string originalMessage = "This is a secret message! 🔒 Hello X25519!";
                var sessionId = Rng.Bytes(16);
                var senderId = X25519Util.Fingerprint16(publicKey);

                results.AppendLine($"   📝 Original message: \"{originalMessage}\"");
                results.AppendLine($"   🆔 Session ID: {Convert.ToHexString(sessionId)[..16]}...");

                // Encrypt message (to self)
                var encryptedPacket = X25519StatelessSession.InitiatorEncrypt(
                    privateKey,      // own private
                    publicKey,       // own public (encrypt to self)
                    sessionId,
                    senderId,
                    1,               // seq = 1
                    Encoding.UTF8.GetBytes(originalMessage)
                );

                results.AppendLine($"   📦 Encrypted packet: {encryptedPacket.Length} bytes");

                // Decrypt message
                var decryptedBytes = X25519StatelessSession.RecipientDecrypt(
                    privateKey,      // own private
                    encryptedPacket,
                    null,           // no extraAad
                    out uint seq,
                    out byte[] sessionIdOut,
                    out byte[] initiatorPubKeyOut,
                    out byte[] senderIdOut
                );

                string decryptedMessage = Encoding.UTF8.GetString(decryptedBytes);

                // Check results
                if (decryptedMessage != originalMessage)
                {
                    results.AppendLine("❌ Decrypted message doesn't match original");
                    results.AppendLine($"   Expected: \"{originalMessage}\"");
                    results.AppendLine($"   Got: \"{decryptedMessage}\"");
                    return false;
                }

                if (seq != 1)
                {
                    results.AppendLine($"❌ Wrong sequence number: {seq}, expected 1");
                    return false;
                }

                if (!ArraysEqual(sessionId, sessionIdOut))
                {
                    results.AppendLine("❌ Session ID doesn't match");
                    return false;
                }

                results.AppendLine($"   ✅ Decrypted message: \"{decryptedMessage}\"");
                results.AppendLine($"   ✅ Sequence: {seq}");
                results.AppendLine("✅ Basic encrypt/decrypt succeeded");
                results.AppendLine();
                return true;
            }
            catch (Exception ex)
            {
                results.AppendLine($"❌ Virhe: {ex.Message}");
                results.AppendLine();
                return false;
            }
        }

        /// <summary>
        /// Test cross-encryption (Alice -> Bob, Bob -> Alice)
        /// </summary>
        public static bool TestCrossKeyEncryption(StringBuilder results)
        {
            results.AppendLine("👥 Test 4: Cross-key communication (Alice ↔ Bob)");
            try
            {
                // Alice and Bob create their own key pairs
                var (alicePriv, alicePub) = X25519Util.Generate();
                var (bobPriv, bobPub) = X25519Util.Generate();

                var sessionId = Rng.Bytes(16);
                var aliceSenderId = X25519Util.Fingerprint16(alicePub);
                var bobSenderId = X25519Util.Fingerprint16(bobPub);

                string aliceMessage = "Hi Bob! This is Alice.";
                string bobMessage = "Hi Alice! This is Bob.";

                results.AppendLine($"   👩 Alice sends: \"{aliceMessage}\"");
                results.AppendLine($"   👨 Bob sends: \"{bobMessage}\"");

                // Alice sends to Bob
                var aliceToBob = X25519StatelessSession.InitiatorEncrypt(
                    alicePriv, bobPub, sessionId, aliceSenderId, 1, 
                    Encoding.UTF8.GetBytes(aliceMessage));

                // Bob sends to Alice
                var bobToAlice = X25519StatelessSession.InitiatorEncrypt(
                    bobPriv, alicePub, sessionId, bobSenderId, 1,
                    Encoding.UTF8.GetBytes(bobMessage));

                // Bob receives Alice's message
                var bobReceived = X25519StatelessSession.RecipientDecrypt(
                    bobPriv, aliceToBob, null, out _, out _, out _, out _);

                // Alice receives Bob's message
                var aliceReceived = X25519StatelessSession.RecipientDecrypt(
                    alicePriv, bobToAlice, null, out _, out _, out _, out _);

                string bobReceivedMsg = Encoding.UTF8.GetString(bobReceived);
                string aliceReceivedMsg = Encoding.UTF8.GetString(aliceReceived);

                if (bobReceivedMsg != aliceMessage)
                {
                    results.AppendLine($"❌ Bob didn't receive Alice's message correctly");
                    return false;
                }

                if (aliceReceivedMsg != bobMessage)
                {
                    results.AppendLine($"❌ Alice didn't receive Bob's message correctly");
                    return false;
                }

                results.AppendLine($"   ✅ Bob received: \"{bobReceivedMsg}\"");
                results.AppendLine($"   ✅ Alice received: \"{aliceReceivedMsg}\"");
                results.AppendLine("✅ Cross-key communication succeeded");
                results.AppendLine();
                return true;
            }
            catch (Exception ex)
            {
                results.AppendLine($"❌ Error: {ex.Message}");
                results.AppendLine();
                return false;
            }
        }

        /// <summary>
        /// Test session management
        /// </summary>
        private static bool TestSessionManagement(StringBuilder results)
        {
            results.AppendLine("📊 Test 5: Session Management");
            try
            {
                var (privateKey, publicKey) = X25519Util.Generate();
                var sessionId = Rng.Bytes(16);
                var senderId = X25519Util.Fingerprint16(publicKey);

                // Send multiple messages in same session
                string[] messages = {
                    "First message",
                    "Second message", 
                    "Third message"
                };

                results.AppendLine($"   📨 Sending {messages.Length} messages in same session...");

                for (uint i = 0; i < messages.Length; i++)
                {
                    var encrypted = X25519StatelessSession.InitiatorEncrypt(
                        privateKey, publicKey, sessionId, senderId, i + 1,
                        Encoding.UTF8.GetBytes(messages[i]));

                    var decrypted = X25519StatelessSession.RecipientDecrypt(
                        privateKey, encrypted, null, out uint seq, out _, out _, out _);

                    string decryptedMsg = Encoding.UTF8.GetString(decrypted);

                    if (decryptedMsg != messages[i])
                    {
                        results.AppendLine($"❌ Message {i + 1} failed");
                        return false;
                    }

                    if (seq != i + 1)
                    {
                        results.AppendLine($"❌ Wrong sequence {i + 1}: {seq}");
                        return false;
                    }

                    results.AppendLine($"   ✅ Message {seq}: \"{decryptedMsg}\"");
                }

                results.AppendLine("✅ Session management succeeded");
                results.AppendLine();
                return true;
            }
            catch (Exception ex)
            {
                results.AppendLine($"❌ Error: {ex.Message}");
                results.AppendLine();
                return false;
            }
        }

        /// <summary>
        /// Test error handling
        /// </summary>
        private static bool TestErrorHandling(StringBuilder results)
        {
            results.AppendLine("⚠️ Test 6: Error handling");
            try
            {
                var (privateKey, publicKey) = X25519Util.Generate();
                var sessionId = Rng.Bytes(16);
                var senderId = X25519Util.Fingerprint16(publicKey);

                // Test corrupted data
                var validPacket = X25519StatelessSession.InitiatorEncrypt(
                    privateKey, publicKey, sessionId, senderId, 1,
                    Encoding.UTF8.GetBytes("Test"));

                // Corrupt packet
                var corruptedPacket = new byte[validPacket.Length];
                Array.Copy(validPacket, corruptedPacket, validPacket.Length);
                corruptedPacket[corruptedPacket.Length / 2] ^= 0xFF; // Flip bits in middle

                try
                {
                    X25519StatelessSession.RecipientDecrypt(
                        privateKey, corruptedPacket, null, out _, out _, out _, out _);
                    results.AppendLine("❌ Corrupted packet didn't cause error");
                    return false;
                }
                catch
                {
                    results.AppendLine("   ✅ Corrupted packet was detected");
                }

                // Test wrong key
                var (wrongPrivateKey, _) = X25519Util.Generate();
                try
                {
                    X25519StatelessSession.RecipientDecrypt(
                        wrongPrivateKey, validPacket, null, out _, out _, out _, out _);
                    results.AppendLine("❌ Wrong key didn't cause error");
                    return false;
                }
                catch
                {
                    results.AppendLine("   ✅ Wrong key was detected");
                }

                // Test empty message
                var emptyPacket = X25519StatelessSession.InitiatorEncrypt(
                    privateKey, publicKey, sessionId, senderId, 1,
                    new byte[0]);

                var emptyDecrypted = X25519StatelessSession.RecipientDecrypt(
                    privateKey, emptyPacket, null, out _, out _, out _, out _);

                if (emptyDecrypted.Length != 0)
                {
                    results.AppendLine("❌ Empty message didn't work correctly");
                    return false;
                }

                results.AppendLine("   ✅ Empty message handled correctly");
                results.AppendLine("✅ Error handling succeeded");
                results.AppendLine();
                return true;
            }
            catch (Exception ex)
            {
                results.AppendLine($"❌ Unexpected error: {ex.Message}");
                results.AppendLine();
                return false;
            }
        }

        /// <summary>
        /// Test performance
        /// </summary>
        private static bool TestPerformance(StringBuilder results)
        {
            results.AppendLine("⚡ Test 7: Performance test");
            try
            {
                var (privateKey, publicKey) = X25519Util.Generate();
                var sessionId = Rng.Bytes(16);
                var senderId = X25519Util.Fingerprint16(publicKey);
                var testMessage = Encoding.UTF8.GetBytes("Performance test - this is a 50 character test message!");

                int iterations = 100;
                results.AppendLine($"   🔄 Running {iterations} encrypt/decrypt rounds...");

                var startTime = DateTime.UtcNow;

                for (int i = 0; i < iterations; i++)
                {
                    var encrypted = X25519StatelessSession.InitiatorEncrypt(
                        privateKey, publicKey, sessionId, senderId, (uint)i + 1, testMessage);

                    var decrypted = X25519StatelessSession.RecipientDecrypt(
                        privateKey, encrypted, null, out _, out _, out _, out _);

                    if (decrypted.Length != testMessage.Length)
                    {
                        results.AppendLine($"❌ Round {i + 1} failed");
                        return false;
                    }
                }

                var endTime = DateTime.UtcNow;
                var totalMs = (endTime - startTime).TotalMilliseconds;
                var avgMs = totalMs / iterations;

                results.AppendLine($"   📊 Total: {totalMs:F1} ms");
                results.AppendLine($"   📊 Average: {avgMs:F2} ms per encrypt+decrypt");
                results.AppendLine($"   📊 Speed: ~{(1000.0 / avgMs):F0} operations/second");

                if (avgMs > 50) // If takes over 50ms per operation
                {
                    results.AppendLine("   ⚠️ Performance slightly slow");
                }
                else if (avgMs < 5) // If under 5ms per operation
                {
                    results.AppendLine("   🚀 Excellent performance!");
                }
                else
                {
                    results.AppendLine("   ✅ Good performance");
                }

                results.AppendLine("✅ Performance test succeeded");
                results.AppendLine();
                return true;
            }
            catch (Exception ex)
            {
                results.AppendLine($"❌ Error: {ex.Message}");
                results.AppendLine();
                return false;
            }
        }

        /// <summary>
        /// Helper function for comparing byte arrays
        /// </summary>
        private static bool ArraysEqual(byte[] a, byte[] b)
        {
            if (a == null || b == null) return a == b;
            if (a.Length != b.Length) return false;
            
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i]) return false;
            }
            return true;
        }
    }
}