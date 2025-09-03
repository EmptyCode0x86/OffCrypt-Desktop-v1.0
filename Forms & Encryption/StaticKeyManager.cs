using System;
using System.Security.Cryptography;
using System.Text;

namespace OffCrypt
{
    /// <summary>
    /// Manages static ECDH keys with Windows DPAPI
    /// Enables decryption of old messages by maintaining the same key
    /// </summary>
    public enum ECDHMode
    {
        StaticP256,     // Static P-256 key (default, old messages work)
        StaticP384,     // Static P-384 key (improved security)
        StaticP521,     // Static P-521 key (maximum security)
        Ephemeral       // Always new key per message (maximum security, no compatibility)
    }

    public static class StaticKeyManager
    {
        private const string REGISTRY_KEY_BASE = @"SOFTWARE\OffCrypt\ECDH";
        
        /// <summary>
        /// Gets or creates a static ECDH key with selected mode
        /// </summary>
        public static ECDiffieHellman GetOrCreateStaticKey(ECDHMode mode)
        {
            if (mode == ECDHMode.Ephemeral)
            {
                // Always create new key in ephemeral mode
                return CreateNewKey(GetCurveForMode(mode));
            }

            try
            {
                // Try to load saved key
                var curve = GetCurveForMode(mode);
                var savedKey = LoadStaticKey(mode);
                
                if (savedKey != null)
                {
                    return savedKey;
                }

                // If key not found, create new and save
                var newKey = CreateNewKey(curve);
                SaveStaticKey(mode, newKey);
                return newKey;
            }
            catch (Exception ex)
            {
                // If static key loading fails, create new
                System.Diagnostics.Debug.WriteLine($"Static key load failed: {ex.Message}");
                var curve = GetCurveForMode(mode);
                var newKey = CreateNewKey(curve);
                
                try
                {
                    SaveStaticKey(mode, newKey);
                }
                catch
                {
                    // If saving fails, still use the created key
                    System.Diagnostics.Debug.WriteLine("Static key save failed, using ephemeral key");
                }
                
                return newKey;
            }
        }

        /// <summary>
        /// Creates new ECDH key with given curve
        /// </summary>
        private static ECDiffieHellman CreateNewKey(ECCurve curve)
        {
            return ECDiffieHellman.Create(curve);
        }

        /// <summary>
        /// Returns ECCurve according to mode
        /// </summary>
        public static ECCurve GetCurveForMode(ECDHMode mode)
        {
            return mode switch
            {
                ECDHMode.StaticP256 or ECDHMode.Ephemeral => ECCurve.NamedCurves.nistP256,
                ECDHMode.StaticP384 => ECCurve.NamedCurves.nistP384,
                ECDHMode.StaticP521 => ECCurve.NamedCurves.nistP521,
                _ => ECCurve.NamedCurves.nistP256
            };
        }

        /// <summary>
        /// Loads saved static key from Windows DPAPI
        /// </summary>
        private static ECDiffieHellman? LoadStaticKey(ECDHMode mode)
        {
            try
            {
                var keyName = GetKeyName(mode);
                
                using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(REGISTRY_KEY_BASE))
                {
                    if (key?.GetValue(keyName) is byte[] encryptedKeyData)
                    {
                        // Decrypt DPAPI protected key
                        byte[] keyData = ProtectedData.Unprotect(
                            encryptedKeyData, 
                            GetEntropy(mode), 
                            DataProtectionScope.CurrentUser);

                        // Create ECDH key from saved data
                        var ecdh = ECDiffieHellman.Create(GetCurveForMode(mode));
                        ecdh.ImportPkcs8PrivateKey(keyData, out _);
                        
                        // Clear sensitive data
                        Array.Clear(keyData, 0, keyData.Length);
                        
                        return ecdh;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load static key: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// Saves static key protected with Windows DPAPI
        /// </summary>
        private static void SaveStaticKey(ECDHMode mode, ECDiffieHellman ecdh)
        {
            try
            {
                var keyName = GetKeyName(mode);
                
                // Export key to PKCS8 format
                byte[] keyData = ecdh.ExportPkcs8PrivateKey();
                
                // Protect with DPAPI
                byte[] encryptedKeyData = ProtectedData.Protect(
                    keyData, 
                    GetEntropy(mode), 
                    DataProtectionScope.CurrentUser);

                // Save to registry
                using (var key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(REGISTRY_KEY_BASE))
                {
                    key.SetValue(keyName, encryptedKeyData, Microsoft.Win32.RegistryValueKind.Binary);
                }

                // Clear sensitive data
                Array.Clear(keyData, 0, keyData.Length);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to save static key: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Returns key name according to mode
        /// </summary>
        private static string GetKeyName(ECDHMode mode)
        {
            return $"StaticKey_{mode}";
        }

        /// <summary>
        /// Returns DPAPI entropy according to mode
        /// </summary>
        private static byte[] GetEntropy(ECDHMode mode)
        {
            return Encoding.UTF8.GetBytes($"OffCrypt_ECDH_{mode}_v1");
        }

        /// <summary>
        /// Deletes saved static key
        /// </summary>
        public static void DeleteStaticKey(ECDHMode mode)
        {
            try
            {
                var keyName = GetKeyName(mode);
                
                using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(REGISTRY_KEY_BASE, true))
                {
                    key?.DeleteValue(keyName, false);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to delete static key: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes all saved static keys
        /// </summary>
        public static void DeleteAllStaticKeys()
        {
            try
            {
                Microsoft.Win32.Registry.CurrentUser.DeleteSubKeyTree(REGISTRY_KEY_BASE, false);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to delete all static keys: {ex.Message}");
            }
        }

        /// <summary>
        /// Checks if static key is saved
        /// </summary>
        public static bool HasStaticKey(ECDHMode mode)
        {
            try
            {
                var keyName = GetKeyName(mode);
                
                using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(REGISTRY_KEY_BASE))
                {
                    return key?.GetValue(keyName) != null;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Returns curve name in user-friendly format
        /// </summary>
        public static string GetCurveDisplayName(ECDHMode mode)
        {
            return mode switch
            {
                ECDHMode.StaticP256 => "NIST P-256 (256-bit)",
                ECDHMode.StaticP384 => "NIST P-384 (384-bit)",
                ECDHMode.StaticP521 => "NIST P-521 (521-bit)",
                ECDHMode.Ephemeral => "P-256 (Ephemeral)",
                _ => "Unknown"
            };
        }
    }
}