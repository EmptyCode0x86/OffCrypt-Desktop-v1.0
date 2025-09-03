using System;
using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;

namespace OffCrypt
{
    /// <summary>
    /// Modernin kryptografian toteutus ECDH + HKDF + AES-GCM
    /// Noudattaa NIST 2024 -suosituksia ja Microsoft Security Guidelines
    /// Sisältää timestamp-tuen katoaville viesteille
    /// </summary>
    public sealed class PasswordSecure : IDisposable
    {
        private const int SALT_SIZE = 32;          // 256-bit suola
        private const int NONCE_SIZE = 12;         // 96-bit nonce AES-GCM:lle
        private const int TAG_SIZE = 16;           // 128-bit autentikointitagi
        private const int KEY_SIZE = 32;           // 256-bit AES-avain
        private const byte VERSION = 0x10;        // Uusi versio modernille toteutukselle

        private readonly ECDiffieHellman _staticEcdh;
        private readonly ECDHMode _ecdhMode;
        private bool _disposed;

        public PasswordSecure() : this(ECDHMode.StaticP256)
        {
        }

        public PasswordSecure(ECDHMode ecdhMode)
        {
            _ecdhMode = ecdhMode;
            _staticEcdh = StaticKeyManager.GetOrCreateStaticKey(ecdhMode);
        }

        /// <summary>
        /// Salaa viestin käyttäen hybridisalausta: ECDH + HKDF + AES-GCM
        /// </summary>
        public string Encrypt(string plaintext, char[] password)
        {
            return EncryptInternal(plaintext, password, VERSION);
        }

        /// <summary>
        /// Salaa viestin custom version bytella (katoaville viesteille)
        /// </summary>
        public string EncryptWithTimestamp(string plaintext, char[] password, byte customVersion)
        {
            return EncryptInternal(plaintext, password, customVersion);
        }

        /// <summary>
        /// Sisäinen salausmetodi joka tukee custom version bytea
        /// </summary>
        private string EncryptInternal(string plaintext, char[] password, byte versionByte)
        {
            ArgumentException.ThrowIfNullOrEmpty(plaintext);
            ArgumentNullException.ThrowIfNull(password);

            if (password.Length == 0)
                throw new ArgumentException("Salasana ei voi olla tyhjä", nameof(password));

            ObjectDisposedException.ThrowIf(_disposed, this);

            // 1. ECDH avainpari salausoperaatiolle:
            //    - Ephemeral mode: Luo uusi ephemeral avainpari
            //    - Static mode: Käytä staattista avainta
            ECDiffieHellman senderEcdh;
            bool disposeSenderEcdh;
            
            if (_ecdhMode == ECDHMode.Ephemeral)
            {
                // Ephemeral: luo aina uusi avainpari
                senderEcdh = ECDiffieHellman.Create(StaticKeyManager.GetCurveForMode(_ecdhMode));
                disposeSenderEcdh = true;
            }
            else
            {
                // Static: käytä staattista avainta
                senderEcdh = _staticEcdh;
                disposeSenderEcdh = false;
            }

            try
            {
                // 2. Vie lähettäjän julkinen avain
                byte[] senderPublicKey = senderEcdh.PublicKey.ExportSubjectPublicKeyInfo();

                // 3. Laske jaettu salaisuus
                //    - Ephemeral: ephemeral private key + static public key
                //    - Static: static private key + static public key (itsensä kanssa)
                byte[] sharedSecret;
                
                if (_ecdhMode == ECDHMode.Ephemeral)
                {
                    // Ephemeral: käytä vastaanottajan staattista julkista avainta
                    sharedSecret = senderEcdh.DeriveKeyFromHash(
                        _staticEcdh.PublicKey,
                        HashAlgorithmName.SHA256);
                }
                else
                {
                    // Static: käytä samaa staattista avainta molemmilla puolilla
                    sharedSecret = senderEcdh.DeriveKeyFromHash(
                        _staticEcdh.PublicKey,
                        HashAlgorithmName.SHA256);
                }

                // 4. Luo suola ja johda avain HKDF:llä
                byte[] salt = RandomNumberGenerator.GetBytes(SALT_SIZE);
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

                // HKDF: Extract-then-Expand paradigma
                // Käytä version bytea ja ECDH-moodia osana info-parametria erottelemaan eri versiot
                string infoString = versionByte == VERSION ? $"OffCrypt-v1-{_ecdhMode}-encrypt" : $"OffCrypt-v{versionByte:X2}-{_ecdhMode}-encrypt";

                byte[] encryptionKey = HKDF.Derive(
                    ikm: CombineArrays(passwordBytes, sharedSecret),
                    salt: salt,
                    info: Encoding.UTF8.GetBytes(infoString),
                    outLen: KEY_SIZE);

                // Tyhjennä herkät tiedot välittömästi
                CryptographicOperations.ZeroMemory(passwordBytes);
                CryptographicOperations.ZeroMemory(sharedSecret);

                try
                {
                    // 5. AES-256-GCM salaus
                    byte[] nonce = RandomNumberGenerator.GetBytes(NONCE_SIZE);
                    byte[] plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
                    byte[] ciphertext = new byte[plaintextBytes.Length];
                    byte[] tag = new byte[TAG_SIZE];

                    // Associated Data: versio + suola (autentikoidaan, ei salata)
                    byte[] associatedData = new byte[1 + SALT_SIZE];
                    associatedData[0] = versionByte; // Käytä custom version bytea
                    salt.CopyTo(associatedData, 1);

                    using var aesGcm = new AesGcm(encryptionKey, TAG_SIZE);
                    aesGcm.Encrypt(nonce, plaintextBytes, ciphertext, tag, associatedData);

                    // 6. Rakenna lopullinen paketti
                    return BuildEncryptedPacket(salt, nonce, tag, senderPublicKey, ciphertext, versionByte);
                }
                finally
                {
                    CryptographicOperations.ZeroMemory(encryptionKey);
                }
            }
            finally
            {
                // Dispose ephemeral key jos se luotiin tälle operaatiolle
                if (disposeSenderEcdh)
                {
                    senderEcdh?.Dispose();
                }
            }
        }

        /// <summary>
        /// Purkaa salatun viestin
        /// </summary>
        public string Decrypt(string encryptedData, char[] password)
        {
            return DecryptInternal(encryptedData, password, false);
        }

        /// <summary>
        /// Purkaa salatun viestin timestamp-tuella (katoaville viesteille)
        /// </summary>
        public string DecryptWithTimestamp(string encryptedData, char[] password)
        {
            return DecryptInternal(encryptedData, password, true);
        }

        /// <summary>
        /// Sisäinen purkumetodi joka tukee sekä normaaleja että timestamp-viestejä
        /// </summary>
        private string DecryptInternal(string encryptedData, char[] password, bool acceptTimestampVersions)
        {
            ArgumentException.ThrowIfNullOrEmpty(encryptedData);
            ArgumentNullException.ThrowIfNull(password);
            ObjectDisposedException.ThrowIf(_disposed, this);

            var (salt, nonce, tag, senderPublicKey, ciphertext, versionByte) = ParseEncryptedPacket(encryptedData);

            // Validoi version byte
            if (!acceptTimestampVersions && versionByte != VERSION)
            {
                throw new NotSupportedException($"Versio {versionByte:X2} ei ole tuettu tavallisessa dekryptauksessa");
            }

            if (acceptTimestampVersions && versionByte != VERSION && versionByte != 0x02 && versionByte != 0x12)
            {
                // 0x02 = legacy disappearing, 0x12 = modern disappearing
                throw new NotSupportedException($"Versio {versionByte:X2} ei ole tuettu timestamp-dekryptauksessa");
            }

            // Tuo lähettäjän julkinen avain ja laske jaettu salaisuus
            byte[] sharedSecret;
            
            if (_ecdhMode == ECDHMode.Ephemeral)
            {
                // Ephemeral mode: Tuo lähettäjän ephemeral julkinen avain ja käytä omaa staattista private keyä
                using var senderPublicKeyEcdh = ECDiffieHellman.Create(StaticKeyManager.GetCurveForMode(_ecdhMode));
                senderPublicKeyEcdh.ImportSubjectPublicKeyInfo(senderPublicKey, out _);
                
                sharedSecret = _staticEcdh.DeriveKeyFromHash(
                    senderPublicKeyEcdh.PublicKey,
                    HashAlgorithmName.SHA256);
            }
            else
            {
                // Static mode: Käytä staattista avainta molemmilla puolilla
                // (lähettäjän julkinen avain on sama kuin oma julkinen avain)
                sharedSecret = _staticEcdh.DeriveKeyFromHash(
                    _staticEcdh.PublicKey,
                    HashAlgorithmName.SHA256);
            }

            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            // Johda sama avain kuin salauksessa käyttäen samaa HKDF info-parametria
            // KRIITTINEN: Info-stringin täytyy olla identtinen salauksen kanssa
            string infoString = versionByte == VERSION ? $"OffCrypt-v1-{_ecdhMode}-encrypt" : $"OffCrypt-v{versionByte:X2}-{_ecdhMode}-encrypt";

            byte[] encryptionKey = HKDF.Derive(
                ikm: CombineArrays(passwordBytes, sharedSecret),
                salt: salt,
                info: Encoding.UTF8.GetBytes(infoString),
                outLen: KEY_SIZE);

            CryptographicOperations.ZeroMemory(passwordBytes);
            CryptographicOperations.ZeroMemory(sharedSecret);

            try
            {
                // Associated Data täytyy olla sama kuin salauksessa
                byte[] associatedData = new byte[1 + SALT_SIZE];
                associatedData[0] = versionByte;
                salt.CopyTo(associatedData, 1);

                byte[] plaintextBytes = new byte[ciphertext.Length];

                using var aesGcm = new AesGcm(encryptionKey, TAG_SIZE);
                aesGcm.Decrypt(nonce, ciphertext, tag, plaintextBytes, associatedData);

                return Encoding.UTF8.GetString(plaintextBytes);
            }
            catch (CryptographicException)
            {
                throw new UnauthorizedAccessException("Väärä salasana tai korruptoitunut data");
            }
            finally
            {
                CryptographicOperations.ZeroMemory(encryptionKey);
            }
        }

        private static byte[] CombineArrays(byte[] first, byte[] second)
        {
            byte[] result = new byte[first.Length + second.Length];
            first.CopyTo(result, 0);
            second.CopyTo(result, first.Length);
            return result;
        }

        private string BuildEncryptedPacket(byte[] salt, byte[] nonce, byte[] tag,
            byte[] senderPublicKey, byte[] ciphertext, byte versionByte)
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);

            // Pakettiformaatti: VERSION | SALT | NONCE | TAG | PUBKEY_LEN | PUBKEY | CIPHERTEXT
            writer.Write(versionByte); // Käytä parametrina saatua version bytea
            writer.Write(salt);
            writer.Write(nonce);
            writer.Write(tag);
            writer.Write(senderPublicKey.Length);
            writer.Write(senderPublicKey);
            writer.Write(ciphertext);

            return Convert.ToBase64String(stream.ToArray());
        }

        private (byte[] salt, byte[] nonce, byte[] tag, byte[] senderPublicKey, byte[] ciphertext, byte versionByte)
            ParseEncryptedPacket(string encryptedData)
        {
            byte[] data = Convert.FromBase64String(encryptedData);
            using var stream = new MemoryStream(data);
            using var reader = new BinaryReader(stream);

            byte version = reader.ReadByte();

            byte[] salt = reader.ReadBytes(SALT_SIZE);
            byte[] nonce = reader.ReadBytes(NONCE_SIZE);
            byte[] tag = reader.ReadBytes(TAG_SIZE);

            int publicKeyLength = reader.ReadInt32();
            if (publicKeyLength < 0 || publicKeyLength > 1024) // Järkevä raja
                throw new InvalidDataException("Virheellinen julkisen avaimen pituus");

            byte[] senderPublicKey = reader.ReadBytes(publicKeyLength);
            byte[] ciphertext = reader.ReadBytes((int)(stream.Length - stream.Position));

            return (salt, nonce, tag, senderPublicKey, ciphertext, version);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                // Huom: StaticKeyManager:in avaimia ei disposata, koska ne ovat jaettuja
                // Vain ephemeral-moodissa luodut avaimet disposataan automaattisesti using-lauseissa
                if (_ecdhMode == ECDHMode.Ephemeral)
                {
                    _staticEcdh?.Dispose();
                }
                _disposed = true;
            }
        }
    }
}