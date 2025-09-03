// ====================================================================================================
// File: X25519StatelessSession.cs
// TargetFramework: net9.0+ 
// Dependency: BouncyCastle (X25519 keys and DER/SPKI/PKCS#8 encoding)
//
// What does this do?
//   • "Stateless" session without ratcheting (no key deletion). Old messages can always be accessed.
//   • One X25519 handshake per session → PSK (Pre-Shared Key) = HKDF(X25519-shared, sessionId, "X25519/PSK").
//   • Initiator's PUBLIC KEY (SPKI) is embedded in each message header.
//   • Each message's key/nonce is derived deterministically from PSK (sessionId + senderId + seq).
//   • Encryption: AES-256-GCM (AEAD). AAD = header || seq || (optional extraAad).
//   • Public and private keys are exported/imported in standard PEM format:
//       -----BEGIN PUBLIC KEY----- / -----END PUBLIC KEY-----     (SPKI, DER)
//       -----BEGIN PRIVATE KEY----- / -----END PRIVATE KEY-----   (PKCS#8, DER)
//
// ====================================================================================================

using System;
using System.Buffers.Binary;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using Org.BouncyCastle.Crypto.Agreement;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

namespace OffCrypt
{
    internal static class Consts
    {
        public const byte Version = 1;

        public const int AesKeyLen   = 32; // 256-bit
        public const int GcmTagLen   = 16; // 128-bit
        public const int GcmNonceLen = 12; // 96-bit

        public const int SessionIdLen = 16; // 128-bit
        public const int SenderIdLen  = 16; // 128-bit

        public const string AlgLabel = "X25519-PSK-A256GCM";

        public const string InfoX25519PSK = "X25519/PSK";
        public const string InfoKey       = "PSK/KEY";
        public const string InfoNonce     = "PSK/NONCE";
    }

    internal static class Rng
    {
        public static byte[] Bytes(int n){ var b=new byte[n]; RandomNumberGenerator.Fill(b); return b; }
    }

    internal static class HashUtil
    {
        public static byte[] Sha256(params byte[][] parts)
        {
            using var sha = SHA256.Create();
            foreach (var p in parts) sha.TransformBlock(p, 0, p.Length, null, 0);
            sha.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
            return sha.Hash!;
        }

        // 128-bit fingerprint (good for senderId): Fingerprint16(publicSpkiDer)
        public static byte[] Fingerprint16(params byte[][] parts) => Sha256(parts).Take(Consts.SenderIdLen).ToArray();
    }

    // RFC 5869 HKDF (HMAC-SHA256)
    public static class HKDF
    {
        public static byte[] Derive(byte[] ikm, byte[] salt, byte[] info, int outLen)
        {
            salt ??= new byte[32];
            using var hExtract = new HMACSHA256(salt);
            var prk = hExtract.ComputeHash(ikm);

            var okm = new byte[outLen];
            var t = Array.Empty<byte>();
            var pos = 0; byte c = 1;

            using var hExpand = new HMACSHA256(prk);
            while (pos < outLen)
            {
                var data = new byte[t.Length + (info?.Length ?? 0) + 1];
                Buffer.BlockCopy(t, 0, data, 0, t.Length);
                if (info != null && info.Length > 0) Buffer.BlockCopy(info, 0, data, t.Length, info.Length);
                data[^1] = c++;
                t = hExpand.ComputeHash(data);
                var take = Math.Min(t.Length, outLen - pos);
                Buffer.BlockCopy(t, 0, okm, pos, take);
                pos += take;
            }
            return okm;
        }
    }

    internal static class AEAD
    {
        public static (byte[] Ct, byte[] Tag) AesGcmEncrypt(byte[] key, byte[] nonce, byte[] plain, byte[] aad)
        {
            var ct = new byte[plain.Length];
            var tag = new byte[Consts.GcmTagLen];
            using var g = new AesGcm(key);
            g.Encrypt(nonce, plain, ct, tag, aad);
            return (ct, tag);
        }

        public static byte[] AesGcmDecrypt(byte[] key, byte[] nonce, byte[] ct, byte[] tag, byte[] aad)
        {
            var p = new byte[ct.Length];
            using var g = new AesGcm(key);
            g.Decrypt(nonce, ct, tag, p, aad);
            return p;
        }
    }

    // Simple len-prefixed writer/reader for binary packet encoding
    internal sealed class BW : IDisposable
    {
        private readonly MemoryStream ms = new();
        public void U8(byte v) => ms.WriteByte(v);
        public void B(byte[] x)
        {
            Span<byte> l = stackalloc byte[4];
            BinaryPrimitives.WriteUInt32BigEndian(l, (uint)(x?.Length ?? 0));
            ms.Write(l);
            if (x != null && x.Length > 0) ms.Write(x, 0, x.Length);
        }
        public void U32(uint v)
        {
            Span<byte> b = stackalloc byte[4];
            BinaryPrimitives.WriteUInt32BigEndian(b, v);
            ms.Write(b);
        }
        public byte[] ToArray() => ms.ToArray();
        public void Dispose()=>ms.Dispose();
    }
    internal ref struct BR
    {
        private ReadOnlySpan<byte> s;
        public BR(ReadOnlySpan<byte> s){ this.s=s; }
        public byte U8(){ var b=s[0]; s=s[1..]; return b; }
        public byte[] B(){ var len=BinaryPrimitives.ReadUInt32BigEndian(s[..4]); s=s[4..]; var v=s[..(int)len].ToArray(); s=s[(int)len..]; return v; }
        public uint U32(){ var v=BinaryPrimitives.ReadUInt32BigEndian(s[..4]); s=s[4..]; return v; }
        public bool EoS => s.IsEmpty;
    }

    // ----------------------------------------------------------------------------------------------------
    // X25519Util – X25519 key pair, shared secret and DER/PEM helpers (SPKI & PKCS#8).
    // PUBLIC KEY PEM = "-----BEGIN PUBLIC KEY-----" (SPKI, DER)
    // PRIVATE KEY PEM = "-----BEGIN PRIVATE KEY-----" (PKCS#8, DER)
    // ----------------------------------------------------------------------------------------------------
    internal static class X25519Util
    {
        public static (byte[] PrivatePkcs8, byte[] PublicSpki) Generate()
        {
            var seed = Rng.Bytes(32);
            var priv = new X25519PrivateKeyParameters(seed, 0);
            var pub  = priv.GeneratePublicKey();

            var pkInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(priv);                  // PKCS#8
            var spki   = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(pub);       // SPKI

            return (pkInfo.GetDerEncoded(), spki.GetDerEncoded());
        }

        public static byte[] PublicFromPrivate(byte[] privatePkcs8)
        {
            var priv = (X25519PrivateKeyParameters) PrivateKeyFactory.CreateKey(privatePkcs8);
            var pub  = priv.GeneratePublicKey();
            return SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(pub).GetDerEncoded();
        }

        public static byte[] DeriveSharedSecret(byte[] myPrivatePkcs8, byte[] peerPublicSpki)
        {
            var priv = (X25519PrivateKeyParameters) PrivateKeyFactory.CreateKey(myPrivatePkcs8);
            var pub  = (X25519PublicKeyParameters)  PublicKeyFactory.CreateKey(peerPublicSpki);

            var agree = new X25519Agreement(); agree.Init(priv);
            var z = new byte[32];
            agree.CalculateAgreement(pub, z, 0);
            return z; // 32B shared secret
        }

        public static string ToPemPublic(byte[] spkiDer)   => PemEncode("PUBLIC KEY",  spkiDer);
        public static string ToPemPrivate(byte[] pkcs8Der) => PemEncode("PRIVATE KEY", pkcs8Der);

        public static byte[] FromPem(string pem, string headerName)
        {
            var head = $"-----BEGIN {headerName}-----";
            var tail = $"-----END {headerName}-----";
            var i = pem.IndexOf(head, StringComparison.Ordinal);
            var j = pem.IndexOf(tail, StringComparison.Ordinal);
            if (i < 0 || j < 0) throw new FormatException("Invalid PEM.");

            var b64 = pem.Substring(i + head.Length, j - (i + head.Length))
                         .Replace("\r","").Replace("\n","").Trim();
            return Convert.FromBase64String(b64);
        }

        private static string PemEncode(string header, byte[] der)
        {
            var b64 = Convert.ToBase64String(der);
            var sb = new StringBuilder();
            sb.AppendLine($"-----BEGIN {header}-----");
            for (int i=0;i<b64.Length;i+=64) sb.AppendLine(b64.Substring(i, Math.Min(64, b64.Length-i)));
            sb.AppendLine($"-----END {header}-----");
            return sb.ToString();
        }

        public static byte[] Fingerprint16(byte[] spkiDer) => HashUtil.Fingerprint16(spkiDer);
    }

    // ----------------------------------------------------------------------------------------------------
    // X25519StatelessSession – INITIATOR ENCRYPT & RECIPIENT DECRYPT
    //
    // Introduction:
    //   • PSK = HKDF( X25519(myPriv, peerPub), salt=sessionId(16B), info="X25519/PSK", len=32 )
    //   • key   = HKDF( PSK, salt=sessionId, info="PSK/KEY|<senderIdHex>|<seq>",   len=32 )
    //   • nonce = HKDF( PSK, salt=sessionId, info="PSK/NONCE|<senderIdHex>|<seq>", len=12 )
    //   → No nonce rotation, deterministic derivation, messages can be opened with PSK anytime.
    //
    // Header fields:
    //   version(1), ALG="X25519-PSK-A256GCM", sessionId(16), initiatorPubSpki, senderId(16)
    //   • "initiatorPubSpki" = initiator's PUBLIC KEY (SPKI, DER) → recipient calculates PSK without separate channel.
    //   • "senderId" = 16B identifier, recommendation: Fingerprint16(initiatorPubSpki) or some persistent sender ID.
    //
    // Requirement:
    //   • Never reuse the same (sessionId, senderId, seq) combination.
    // ----------------------------------------------------------------------------------------------------
    public static class X25519StatelessSession
    {
        public static byte[] InitiatorEncrypt(
            byte[] initiatorPrivatePkcs8,   // initiator's PRIVATE (PKCS#8 DER)
            byte[] recipientPublicSpki,     // recipient's PUBLIC (SPKI DER)
            byte[] sessionId16,             // 16B random session ID (keep per conversation)
            byte[] senderId16,              // 16B sender ID (e.g. initiator's public fingerprint)
            uint seq,                       // monotonically increasing message counter per sender
            byte[] plaintext,               // plaintext
            byte[] extraAad = null          // optional AAD (bound to integrity protection)
        )
        {
            if (sessionId16.Length != Consts.SessionIdLen) throw new ArgumentException("sessionId must be 16 bytes.");
            if (senderId16.Length  != Consts.SenderIdLen)  throw new ArgumentException("senderId must be 16 bytes.");

            // 1) PSK from X25519 handshake
            var z   = X25519Util.DeriveSharedSecret(initiatorPrivatePkcs8, recipientPublicSpki);
            var psk = HKDF.Derive(z, sessionId16, Encoding.UTF8.GetBytes(Consts.InfoX25519PSK), Consts.AesKeyLen);

            // 2) Initiator's PUBLIC KEY (SPKI) in header
            var initiatorPubSpki = X25519Util.PublicFromPrivate(initiatorPrivatePkcs8);

            using var headerW = new BW();
            headerW.U8(Consts.Version);
            headerW.B(Encoding.UTF8.GetBytes(Consts.AlgLabel));
            headerW.B(sessionId16);
            headerW.B(initiatorPubSpki);
            headerW.B(senderId16);
            var header = headerW.ToArray();

            // 3) Per-message key/nonce
            Span<byte> seq4 = stackalloc byte[4];
            BinaryPrimitives.WriteUInt32BigEndian(seq4, seq);

            var keyInfo   = Encoding.UTF8.GetBytes($"{Consts.InfoKey}|{Convert.ToHexString(senderId16)}|{seq}");
            var nonceInfo = Encoding.UTF8.GetBytes($"{Consts.InfoNonce}|{Convert.ToHexString(senderId16)}|{seq}");

            var key   = HKDF.Derive(psk, sessionId16, keyInfo,   Consts.AesKeyLen);
            var nonce = HKDF.Derive(psk, sessionId16, nonceInfo, Consts.GcmNonceLen);

            // 4) AAD = header || seq || extraAad
            using var aadW = new BW();
            aadW.B(header);
            aadW.B(seq4.ToArray());
            aadW.B(extraAad ?? Array.Empty<byte>());
            var aad = aadW.ToArray();

            var (ct, tag) = AEAD.AesGcmEncrypt(key, nonce, plaintext, aad);

            using var outW = new BW();
            outW.B(header);
            outW.B(seq4.ToArray());
            outW.B(ct);
            outW.B(tag);
            return outW.ToArray();
        }

        public static byte[] RecipientDecrypt(
            byte[] recipientPrivatePkcs8,   // recipient's PRIVATE (PKCS#8 DER)
            byte[] packet,                  // complete message packet
            byte[] extraAad,                // same extraAad, if used
            out uint seqOut,                // returns message seq
            out byte[] sessionIdOut,        // returns sessionId(16)
            out byte[] initiatorPubSpkiOut, // returns initiator's PUBLIC (SPKI)
            out byte[] senderIdOut          // returns senderId(16)
        )
        {
            var br = new BR(packet);
            var header = br.B();
            var seqB   = br.B();
            var ct     = br.B();
            var tag    = br.B();

            var hr = new BR(header);
            var v   = hr.U8(); _ = v;
            var alg = Encoding.UTF8.GetString(hr.B()); if (alg != Consts.AlgLabel) throw new CryptographicException("Algorithm mismatch.");
            var sess = hr.B(); sessionIdOut = sess;
            var initSpki = hr.B(); initiatorPubSpkiOut = initSpki;
            var sid  = hr.B(); senderIdOut = sid;

            if (sess.Length != Consts.SessionIdLen) throw new CryptographicException("Bad sessionId length.");
            if (sid.Length  != Consts.SenderIdLen)  throw new CryptographicException("Bad senderId length.");

            var seq = BinaryPrimitives.ReadUInt32BigEndian(seqB); seqOut = seq;

            // Calculate PSK from X25519
            var z   = X25519Util.DeriveSharedSecret(recipientPrivatePkcs8, initSpki);
            var psk = HKDF.Derive(z, sess, Encoding.UTF8.GetBytes(Consts.InfoX25519PSK), Consts.AesKeyLen);

            // Derive key/nonce deterministically
            var keyInfo   = Encoding.UTF8.GetBytes($"{Consts.InfoKey}|{Convert.ToHexString(sid)}|{seq}");
            var nonceInfo = Encoding.UTF8.GetBytes($"{Consts.InfoNonce}|{Convert.ToHexString(sid)}|{seq}");

            var key   = HKDF.Derive(psk, sess, keyInfo,   Consts.AesKeyLen);
            var nonce = HKDF.Derive(psk, sess, nonceInfo, Consts.GcmNonceLen);

            using var aadW = new BW();
            aadW.B(header);
            aadW.B(seqB);
            aadW.B(extraAad ?? Array.Empty<byte>());
            var aad = aadW.ToArray();

            return AEAD.AesGcmDecrypt(key, nonce, ct, tag, aad);
        }
    }
}