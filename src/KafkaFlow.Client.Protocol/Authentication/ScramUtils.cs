namespace KafkaFlow.Client.Protocol.Authentication
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    public static class ScramUtils
    {
        public static byte[] Hi(string password, string salt, int count)
        {
            var saltBytes = Convert.FromBase64String(salt);
            using var db = new Rfc2898DeriveBytes(password, saltBytes, count, HashAlgorithmName.SHA512);

            return db.GetBytes(64);
        }

        public static byte[] HMAC(byte[] key, string data)
        {
            using var hmac = new HMACSHA512(key);
            return hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        }

        public static byte[] H(byte[] data)
        {
            using var sha = new SHA512Managed();
            return sha.ComputeHash(data);
        }

        public static byte[] Xor(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
            {
                throw new InvalidOperationException();
            }

            var ret = new byte[a.Length];

            for (var i = 0; i < a.Length; i++)
            {
                ret[i] = (byte)(a[i] ^ b[i]);
            }

            return ret;
        }
    }
}
