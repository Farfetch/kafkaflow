namespace KafkaFlow.Client.Protocol.Authentication.SASL.Scram
{
    using System;
    using System.Linq;
    using System.Security.Authentication;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using KafkaFlow.Client.Protocol.Messages.Implementations.SaslAuthenticate;
    using KafkaFlow.Client.Protocol.Messages.Implementations.SaslHandshake;

    public abstract class BaseScramAuthentication : IAuthenticationMethod
    {
        private static readonly Regex FirstServerMessagePattern = new(
            @"r=(?<nonce>[\w+/=]+),s=(?<salt>[\w+/=]+),i=(?<iteration>[\d]+)",
            RegexOptions.None);

        private static readonly Regex FinalServerMessagePattern = new(
            @"v=(?<signature>[\w+/=]+)",
            RegexOptions.None);

        private readonly string username;
        private readonly string password;
        private readonly string scramMechanism;
        private readonly IScramHashMethods scramHashMethods;

        private string authMessage;
        private byte[] saltedPassword;

        protected BaseScramAuthentication(
            string username,
            string password,
            string scramMechanism,
            IScramHashMethods scramHashMethods)
        {
            this.username = username;
            this.password = password;
            this.scramMechanism = scramMechanism;
            this.scramHashMethods = scramHashMethods;
        }

        public async Task<long> AuthenticateAsync(IBrokerConnection connection)
        {
            var handshakeResponse = await connection.SendAsync(new SaslHandshakeRequestV1(this.scramMechanism));

            if (handshakeResponse.ErrorCode != 0)
            {
                throw new AuthenticationException(
                    $"Authentication failed: the server only supports {string.Join(',', handshakeResponse.Mechanisms)} auth mechanisms");
            }

            var clientFirstMessage = new ClientFirstMessage(this.username);
            var serverFirstAuthResponse = await connection.SendAsync(new SaslAuthenticateRequestV2(clientFirstMessage.GetBytes()));

            if (serverFirstAuthResponse.ErrorCode != 0)
            {
                throw new AuthenticationException(
                    $"Authentication failed: Code={serverFirstAuthResponse.ErrorCode}, Message={serverFirstAuthResponse.ErrorMessage}");
            }

            var serverFirstRaw = Encoding.UTF8.GetString(serverFirstAuthResponse.AuthBytes);
            var firstServerMessageResult = FirstServerMessagePattern.Match(serverFirstRaw);

            if (!firstServerMessageResult.Success)
            {
                throw new AuthenticationException("Authentication failed: Unexpected server response");
            }

            var firstServerMessage = new ServerFirstMessage(
                firstServerMessageResult.Groups["nonce"].Value,
                firstServerMessageResult.Groups["salt"].Value,
                int.Parse(firstServerMessageResult.Groups["iteration"].Value));

            if (!firstServerMessage.Nonce.StartsWith(clientFirstMessage.Nonce))
            {
                throw new AuthenticationException("Authentication failed: Invalid server Nonce");
            }

            var clientLastMessageBytes = this.BuildClientLastMessage(
                clientFirstMessage.GetGs2Header(),
                clientFirstMessage.GetBareData(),
                firstServerMessage.Nonce,
                firstServerMessage.Salt,
                firstServerMessage.Iteration,
                serverFirstRaw,
                this.password);

            var serverFinalAuthResponse = await connection.SendAsync(new SaslAuthenticateRequestV2(clientLastMessageBytes));

            if (serverFinalAuthResponse.ErrorCode != 0)
            {
                throw new AuthenticationException(
                    $"Authentication failed: Code={serverFinalAuthResponse.ErrorCode}, Message={serverFinalAuthResponse.ErrorMessage}");
            }

            this.AuthenticateServer(serverFinalAuthResponse.AuthBytes);

            return serverFinalAuthResponse.SessionLifetimeMs;
        }

        private void AuthenticateServer(byte[] rawMessage)
        {
            var message = Encoding.UTF8.GetString(rawMessage);

            var result = FinalServerMessagePattern.Match(message);

            if (!result.Success)
            {
                throw new AuthenticationException("Authentication failed: Invalid server final response");
            }

            var serverSignature = Convert.FromBase64String(result.Groups["signature"].Value);

            var serverKey = this.scramHashMethods.HMAC(this.saltedPassword, "Server Key");
            var calculatedSignature = this.scramHashMethods.HMAC(serverKey, this.authMessage);

            if (!serverSignature.SequenceEqual(calculatedSignature))
            {
                throw new AuthenticationException("Authentication failed: Invalid server signature");
            }
        }

        private byte[] BuildClientLastMessage(
            string gs2Header,
            string clientBareData,
            string serverNonce,
            string salt,
            int iteration,
            string serverFirstRaw,
            string password)
        {
            var base64Gs2Header = Convert.ToBase64String(Encoding.UTF8.GetBytes(gs2Header));
            var withoutProof = $"c={base64Gs2Header},r={serverNonce}";
            this.authMessage = $"{clientBareData},{serverFirstRaw},{withoutProof}";
            this.saltedPassword = this.scramHashMethods.Hi(password, salt, iteration);
            var clientKey = this.scramHashMethods.HMAC(this.saltedPassword, "Client Key");
            var storedKey = this.scramHashMethods.H(clientKey);
            var clientSignature = this.scramHashMethods.HMAC(storedKey, this.authMessage);
            var clientProof = Xor(clientKey, clientSignature);

            var clientLastMessage = $"{withoutProof},p={Convert.ToBase64String(clientProof)}";
            var clientLastMessageBytes = Encoding.UTF8.GetBytes(clientLastMessage);
            return clientLastMessageBytes;
        }

        private static byte[] Xor(byte[] a, byte[] b)
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

        private class ClientFirstMessage
        {
            public ClientFirstMessage(string username)
            {
                this.Username = username;
                this.Nonce = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                this.AuthorizationId = string.Empty;
            }

            public string Nonce { get; }

            public string AuthorizationId { get; }

            public string Username { get; }

            public string GetBareData() => $"n={this.Username},r={this.Nonce}";

            public string GetGs2Header() => $"n,{this.AuthorizationId},";

            public byte[] GetBytes() => Encoding.UTF8.GetBytes($"{this.GetGs2Header()}{this.GetBareData()}");
        }

        private class ServerFirstMessage
        {
            public ServerFirstMessage(string nonce, string salt, int iteration)
            {
                this.Nonce = nonce;
                this.Salt = salt;
                this.Iteration = iteration;
            }

            public string Nonce { get; }

            public string Salt { get; }

            public int Iteration { get; }
        }
    }
}
