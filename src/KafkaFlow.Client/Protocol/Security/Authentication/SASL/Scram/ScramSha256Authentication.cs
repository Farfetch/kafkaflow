namespace KafkaFlow.Client.Protocol.Security.Authentication.SASL.Scram
{
    using System;
    using global::System.Security.Cryptography;
    using global::System.Text;

    public class ScramSha256Authentication : BaseScramAuthentication
    {
        public ScramSha256Authentication(string username, string password)
            : base(username, password, "SCRAM-SHA-256", ScramSha256Methods.Instance)
        {
        }

        private class ScramSha256Methods : IScramHashMethods
        {
            public static readonly IScramHashMethods Instance = new ScramSha256Methods();

            private ScramSha256Methods()
            {
            }

            public byte[] Hi(string password, string salt, int count)
            {
                var saltBytes = Convert.FromBase64String(salt);
                using var db = new Rfc2898DeriveBytes(password, saltBytes, count, HashAlgorithmName.SHA256);

                return db.GetBytes(32);
            }

            public byte[] HMAC(byte[] key, string data)
            {
                using var hmac = new HMACSHA256(key);
                return hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            }

            public byte[] H(byte[] data)
            {
                using var sha = new SHA256Managed();
                return sha.ComputeHash(data);
            }
        }
    }
}
