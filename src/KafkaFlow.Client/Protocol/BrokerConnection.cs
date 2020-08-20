namespace KafkaFlow.Client.Protocol
{
    using System;
    using System.Buffers.Binary;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Net.Sockets;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using KafkaFlow.Client.Exceptions;
    using KafkaFlow.Client.Protocol.Messages;
    using KafkaFlow.Client.Protocol.Security;
    using KafkaFlow.Client.Protocol.Streams;

    /// <summary>
    /// Represents and manage a broker connection
    /// </summary>
    public class BrokerConnection : IBrokerConnection, IInternalBrokerConnection
    {
        private readonly SemaphoreSlim authenticationSemaphore = new(1, 1);
        private readonly Dictionary<int, PendingRequest> pendingRequests = new();
        private readonly CancellationTokenSource stopTokenSource = new();
        private readonly byte[] messageSizeBuffer = new byte[sizeof(int)];

        private readonly string clientId;
        private readonly TimeSpan requestTimeout;
        private readonly ISecurityProtocol securityProtocol;

        private readonly TcpClient client;
        private readonly Stream stream;
        private readonly Task listenerTask;

        private DateTime sessionExpiryTime = DateTime.MinValue;

        private volatile int lastCorrelationId;

        /// <summary>
        /// Initializes a new instance of the <see cref="BrokerConnection"/> class.
        /// </summary>
        /// <param name="address">The broker address</param>
        /// <param name="clientId">The application client id</param>
        /// <param name="requestTimeout">The connection request timeout</param>
        /// <param name="securityProtocol">The security information to be used on the connection</param>
        public BrokerConnection(
            BrokerAddress address,
            string clientId,
            TimeSpan requestTimeout,
            ISecurityProtocol? securityProtocol = null)
        {
            this.Address = address;
            this.clientId = clientId;
            this.requestTimeout = requestTimeout;
            this.securityProtocol = securityProtocol ?? NullSecurityProtocol.Instance;

            this.client = new TcpClient(address.Host, address.Port);
            this.stream = this.securityProtocol.CreateSecureStream(this.client.GetStream());
            this.listenerTask = Task.Run(this.ListenStream);
        }

        /// <inheritdoc cref="IBrokerConnection.Address"/>
        public BrokerAddress Address { get; }

        /// <inheritdoc/>
        public async Task<TResponse> SendAsync<TResponse>(IRequestMessage<TResponse> request)
            where TResponse : class, IResponse
        {
            await this.EnsureAuthenticationAsync();
            return await ((IInternalBrokerConnection)this).SendAsync(request);
        }

        Task<TResponse> IInternalBrokerConnection.SendAsync<TResponse>(IRequestMessage<TResponse> request)
        {
            var pendingRequest = new PendingRequest(this.requestTimeout, request.ResponseType);

            using var tmp = new MemoryWriter();

            var correlationId = Interlocked.Increment(ref this.lastCorrelationId);

            this.pendingRequests.Add(correlationId, pendingRequest);

            tmp.WriteMessage(new Request(correlationId, this.clientId, request));
            tmp.Position = 0;

#if DEBUG
            var bytes = tmp.ToArray();
            Debug.WriteLine($"{request.GetType().Name}: {string.Join(",", bytes)}");
#endif

            lock (this.stream)
            {
                tmp.CopyTo(this.stream);
            }

            return pendingRequest.GetTask<TResponse>();
        }

        /// <inheritdoc cref="IAsyncDisposable.DisposeAsync"/>>
        public async ValueTask DisposeAsync()
        {
            this.stopTokenSource.Cancel();
            await this.listenerTask.ConfigureAwait(false);

            this.stopTokenSource.Dispose();
            this.listenerTask.Dispose();
            this.stream.Dispose();
            this.client.Dispose();
        }

        private async Task ListenStream()
        {
            while (!this.stopTokenSource.IsCancellationRequested)
            {
                try
                {
                    var messageSize = await this.WaitForMessageSizeAsync().ConfigureAwait(false);

                    Debug.WriteLine($"Received message with {messageSize}b size");

                    if (messageSize <= 0)
                    {
                        continue;
                    }

                    using var memoryStream = new MemoryReader(messageSize);
                    await memoryStream.ReadFromAsync(this.stream).ConfigureAwait(false);
                    memoryStream.Position = 0;
                    this.RespondMessage(memoryStream);
                }
                catch (OperationCanceledException)
                {
                    return;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RespondMessage(MemoryReader source)
        {
            var correlationId = source.ReadInt32();

            if (!this.pendingRequests.TryGetValue(correlationId, out var request))
            {
                Debug.WriteLine($"Received Invalid message CID: {correlationId}");
                return;
            }

            Debug.WriteLine($"Received CID: {correlationId}, Type {request.ResponseType.Name} with {source.Length:N0}b");

            this.pendingRequests.Remove(correlationId);

            var response = (IResponse)Activator.CreateInstance(request.ResponseType);

            if (response is ITaggedFields)
            {
                _ = source.ReadTaggedFields();
            }

            response.Read(source);

            if (source.Length != source.Position)
            {
                throw new KafkaClientException("Some data was not read from response");
            }

            request.CompletionSource.TrySetResult(response);
        }

        private async Task<int> WaitForMessageSizeAsync()
        {
            while (!this.stopTokenSource.IsCancellationRequested)
            {
                if (this.stream.CanRead)
                {
                    var read = await this.stream
                        .ReadAsync(this.messageSizeBuffer, 0, sizeof(int), this.stopTokenSource.Token)
                        .ConfigureAwait(false);

                    if (read > 0)
                    {
                        break;
                    }
                }

                await Task.Delay(100).ConfigureAwait(false);
            }

            return BinaryPrimitives.ReadInt32BigEndian(this.messageSizeBuffer);
        }

        private async ValueTask EnsureAuthenticationAsync()
        {
            if (this.sessionExpiryTime >= DateTime.Now)
            {
                return;
            }

            await this.authenticationSemaphore.WaitAsync().ConfigureAwait(false);

            try
            {
                if (this.sessionExpiryTime < DateTime.Now)
                {
                    var sessionLifetime = await this.securityProtocol
                        .AuthenticateAsync(this)
                        .ConfigureAwait(false);

                    this.sessionExpiryTime = sessionLifetime == 0 ?
                        DateTime.MaxValue :
                        DateTime.Now.AddMilliseconds(sessionLifetime);
                }
            }
            finally
            {
                this.authenticationSemaphore.Release();
            }
        }

        private readonly struct PendingRequest
        {
            public readonly TaskCompletionSource<IResponse> CompletionSource;

            public PendingRequest(TimeSpan timeout, Type responseType)
            {
                this.Timeout = timeout;
                this.ResponseType = responseType;
                this.CompletionSource = new TaskCompletionSource<IResponse>();
            }

            public TimeSpan Timeout { get; }

            public Type ResponseType { get; }

            public Task<TResponse> GetTask<TResponse>()
                where TResponse : IResponse =>
                this.CompletionSource.Task.ContinueWith(x => (TResponse)x.Result);
        }
    }
}
