namespace KafkaFlow.Client.Protocol.Security.Authentication.SASL.Scram
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    public class ScramSha512Authentication : BaseScramAuthentication
    {
        public ScramSha512Authentication(string username, string password)
            : base(username, password, "SCRAM-SHA-512", ScramSha512Methods.Instance)
        {
        }

        private class ScramSha512Methods : IScramHashMethods
        {
            public static readonly IScramHashMethods Instance = new ScramSha512Methods();

            private ScramSha512Methods()
            {
            }

            public byte[] Hi(string password, string salt, int count)
            {
                var saltBytes = Convert.FromBase64String(salt);
                using var db = new Rfc2898DeriveBytes(password, saltBytes, count, HashAlgorithmName.SHA512);

                return db.GetBytes(64);
            }

            public byte[] HMAC(byte[] key, string data)
            {
                using var hmac = new HMACSHA512(key);
                return hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            }

            public byte[] H(byte[] data)
            {
                using var sha = new SHA512Managed();
                return sha.ComputeHash(data);
            }
        }
    }
}
