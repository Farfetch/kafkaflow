namespace KafkaFlow.Client.Sample
{
    using System;
    using System.Linq;
    using System.Security.Authentication;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using KafkaFlow.Client.Protocol;
    using KafkaFlow.Client.Protocol.Authentication;
    using KafkaFlow.Client.Protocol.Messages;
    using KafkaFlow.Client.Protocol.Messages.Implementations.ApiVersion;
    using KafkaFlow.Client.Protocol.Messages.Implementations.Fetch;
    using KafkaFlow.Client.Protocol.Messages.Implementations.Metadata;
    using KafkaFlow.Client.Protocol.Messages.Implementations.OffsetFetch;
    using KafkaFlow.Client.Protocol.Messages.Implementations.Produce;
    using KafkaFlow.Client.Protocol.Messages.Implementations.SaslAuthenticate;
    using KafkaFlow.Client.Protocol.Messages.Implementations.SaslHandshake;

    class Program
    {
        private static readonly Regex FirstServerMessagePattern = new Regex(
            @"r=(?<nonce>[\w+/=]+),s=(?<salt>[\w+/=]+),i=(?<iteration>[\d]+)",
            RegexOptions.None);

        static async Task Main(string[] args)
        {
            const string groupId = "print-console-handler-1";
            const string topicName = "test-client";
            string broker = "localhost";

            var connection = new BrokerConnection(
                new BrokerAddress(broker, 9092),
                "test-client-id",
                TimeSpan.FromSeconds(30));

            // var apiVersion = await connection.SendAsync(new ApiVersionV2Request());

            var username = "alice";
            var password = "alice-pwd";

            var handshakeResponse = await connection.SendAsync(new SaslHandshakeRequestV1("SCRAM-SHA-512"));

            if (handshakeResponse.ErrorCode != 0)
            {
                throw new AuthenticationException(
                    $"Authentication failed: the server only supports {string.Join(',', handshakeResponse.Mechanisms)} auth mechanisms");
            }

            var clientFirstMessage = new ClientFirstMessage(username);
            var authResponse = await connection.SendAsync(new SaslAuthenticateRequestV2(clientFirstMessage.GetBytes()));

            if (authResponse.ErrorCode != 0)
            {
                throw new AuthenticationException(
                    $"Authentication failed: Code={authResponse.ErrorCode}, Message={authResponse.ErrorMessage}");
            }

            var serverFirstRaw = Encoding.UTF8.GetString(authResponse.AuthBytes);
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

            var clientLastMessageBytes = GetClientLastMessage(
                clientFirstMessage.GetGs2Header(),
                clientFirstMessage.GetBareData(),
                firstServerMessage.Nonce,
                firstServerMessage.Salt,
                firstServerMessage.Iteration,
                serverFirstRaw,
                password);

            // var clientLastMessageBytes = GetClientLastMessage(
            //     "n,,",
            //     "n=user,r=fyko+d2lbbFgONRv9qkxdawL",
            //     "fyko+d2lbbFgONRv9qkxdawL3rfcNHYJY1ZVvWVs7j",
            //     "QSXCR+Q6sek8bf92",
            //     4096,
            //     "r=fyko+d2lbbFgONRv9qkxdawL3rfcNHYJY1ZVvWVs7j,s=QSXCR+Q6sek8bf92,i=4096",
            //     "pencil");

            var authResponse2 = await connection.SendAsync(new SaslAuthenticateRequestV2(clientLastMessageBytes));

            Console.WriteLine("Ended!");

            /*            
            var topicMetadata = await connection.SendAsync(
                new MetadataV9Request
                {
                    Topics = new IMetadataRequest.ITopic[]
                    {
                        new MetadataV9Request.Topic { Name = topicName }
                    }
                });
                
            var findCoordResponse = await connection.SendAsync(
                new FindCoordinatorV3Request(string.Empty, 0));

            var joinGroupResponse = await connection.SendAsync(
                new JoinGroupV7Request
                {
                    GroupId = "print-console-handler",
                    SessionTimeoutMs = 300000,
                    RebalanceTimeoutMs = 3000,
                    MemberId = string.Empty,
                    ProtocolType = "consumer",
                    SupportedProtocols = new IJoinGroupRequest.IProtocol[]
                    {
                        new JoinGroupV7Request.Protocol { Name = "consumer", Metadata = Array.Empty<byte>() }
                    }
                });

            var joinGroupResponse1 = await connection.SendAsync(
                new JoinGroupV7Request
                {
                    GroupId = "print-console-handler",
                    SessionTimeoutMs = 300000,
                    RebalanceTimeoutMs = 3000,
                    MemberId = joinGroupResponse.MemberId,
                    ProtocolType = "consumer",
                    SupportedProtocols = new IJoinGroupRequest.IProtocol[]
                    {
                        new JoinGroupV7Request.Protocol { Name = "consumer", Metadata = Array.Empty<byte>() }
                    }
                });

            var heartbeatResponse = await connection.SendAsync(
                new HeartbeatV4Request(
                    groupId,
                    joinGroupResponse1.GenerationId,
                    joinGroupResponse1.MemberId));

            var partitions = topicMetadata
                .Topics
                .First(t => t.Name == topicName)
                .Partitions
                .Select(p => p.Id)
                .ToArray();

            var committedOffsets = await connection.SendAsync(new OffsetFetchV5Request(groupId, topicName, partitions));


            var lastOffsets = await connection.SendAsync(new ListOffsetsV5Request(-1, 0, topicName, partitions));

*/

            //var produceResponse = await ProduceMessage(connection);
            //produceResponse = await MassProduceMessage(connection);
            //var fetchResponse = await FetchMessage(connection);

            Console.ReadLine();
        }

        private static byte[] GetClientLastMessage(
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
            var authMessage = $"{clientBareData},{serverFirstRaw},{withoutProof}";
            var saltedPassword = ScramUtils.Hi(password, salt, iteration);
            var clientKey = ScramUtils.HMAC(saltedPassword, "Client Key");
            var storedKey = ScramUtils.H(clientKey);
            var clientSignature = ScramUtils.HMAC(storedKey, authMessage);
            var clientProof = ScramUtils.Xor(clientKey, clientSignature);

            var clientLastMessage = $"{withoutProof},p={Convert.ToBase64String(clientProof)}";
            var clientLastMessageBytes = Encoding.UTF8.GetBytes(clientLastMessage);
            return clientLastMessageBytes;
        }

        private static Task<FetchV11Response> FetchMessage(IBrokerConnection connection)
        {
            return connection.SendAsync(
                new FetchV11Request
                {
                    ReplicaId = -1,
                    MaxWaitTime = 5000,
                    MinBytes = 0,
                    MaxBytes = 1024 * 16 * 3,
                    IsolationLevel = 1,
                    Topics = new[]
                    {
                        new FetchV11Request.Topic
                        {
                            Name = "test-client",
                            Partitions = new[]
                            {
                                new FetchV11Request.Partition
                                {
                                    Id = 0,
                                    FetchOffset = 0,
                                    PartitionMaxBytes = 1024 * 16
                                },
                                new FetchV11Request.Partition
                                {
                                    Id = 1,
                                    FetchOffset = 0,
                                    PartitionMaxBytes = 1024 * 16
                                },
                                new FetchV11Request.Partition
                                {
                                    Id = 2,
                                    FetchOffset = 0,
                                    PartitionMaxBytes = 1024 * 16
                                },
                            }
                        }
                    }
                });
        }

        private static Task<IProduceResponse> ProduceMessage(IBrokerConnection connection)
        {
            var batch = new RecordBatch();

            var topic = new ProduceV8Request.Topic("test-client");

            var headers = new Headers();

            var timestamp = 1626732933960;

            headers.Add("teste_header_key", Encoding.UTF8.GetBytes("teste_header_value"));

            topic.Partitions.TryAdd(
                0,
                new ProduceV8Request.Partition(0)
                {
                    RecordBatch = batch
                });

            batch.AddRecord(
                new RecordBatch.Record
                {
                    Key = Encoding.UTF8.GetBytes("teste_key"),
                    Value = Encoding.UTF8.GetBytes("teste_value"),
                    Headers = headers
                },
                timestamp);

            var request = new ProduceV8Request(ProduceAcks.Leader, 5000);

            request.Topics.TryAdd(topic.Name, topic);

            return connection.SendAsync(request);
        }

        private static Task<IProduceResponse> MassProduceMessage(IBrokerConnection connection)
        {
            var topic = new ProduceV8Request.Topic("test-client");

            var headers = new Headers();

            headers.Add("teste_header_key", Encoding.UTF8.GetBytes("teste_header_value"));

            var timestamp = 1626732933960;

            for (var partition = 0; partition < 6; partition++)
            {
                var batch = new RecordBatch();

                topic.Partitions.TryAdd(
                    partition,
                    new ProduceV8Request.Partition(partition)
                    {
                        RecordBatch = batch
                    });

                for (var msg = 0; msg < 100; msg++)
                {
                    batch.AddRecord(
                        new RecordBatch.Record
                        {
                            Key = Encoding.UTF8.GetBytes($"teste_key_{msg}"),
                            Value = Encoding.UTF8.GetBytes($"teste_value{msg}"),
                            Headers = headers
                        },
                        timestamp + msg);
                }
            }

            var request = new ProduceV8Request(ProduceAcks.Leader, 5000);

            request.Topics.TryAdd(topic.Name, topic);

            return connection.SendAsync(request);
        }
    }

    internal class ClientFirstMessage
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

    internal class ServerFirstMessage
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
