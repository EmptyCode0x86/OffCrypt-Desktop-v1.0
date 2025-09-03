using System;
using System.Text;

namespace OffCrypt
{
    public static class ArmorUtil
    {
        public static string Encode(string label, byte[] payload)
        {
            string b64 = Convert.ToBase64String(payload);
            var sb = new StringBuilder();
            sb.AppendLine($"-----BEGIN {label}-----");
            for (int i = 0; i < b64.Length; i += 64)
                sb.AppendLine(b64.Substring(i, Math.Min(64, b64.Length - i)));
            sb.AppendLine($"-----END {label}-----");
            return sb.ToString();
        }

        public static byte[] Decode(string armored, string label)
        {
            string begin = $"-----BEGIN {label}-----";
            string end = $"-----END {label}-----";
            int i1 = armored.IndexOf(begin, StringComparison.Ordinal);
            int i2 = armored.IndexOf(end, StringComparison.Ordinal);
            if (i1 < 0 || i2 < 0 || i2 <= i1) throw new FormatException("Invalid armor");
            string b64 = armored.Substring(i1 + begin.Length, i2 - (i1 + begin.Length))
                              .Replace("\r", "").Replace("\n", "").Trim();
            return Convert.FromBase64String(b64);
        }
    }
}
