namespace KafkaFlow.Client.Protocol
{
    using System;
    using System.Buffers.Binary;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.IO;
    using System.Net.Sockets;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using KafkaFlow.Client.Protocol.Messages;
    using KafkaFlow.Client.Protocol.Streams;

    /// <summary>
    /// Represents a TCP connection to a Kafka broker
    /// </summary>
    public class BrokerConnection : IBrokerConnection
    {
        private readonly TcpClient client;
        private readonly NetworkStream stream;
        private readonly ConcurrentDictionary<int, PendingRequest> waitingResponse = new();
        private readonly CancellationTokenSource stopTokenSource = new();
        private readonly Task listenerTask;
        private readonly byte[] messageSizeBuffer = new byte[sizeof(int)];
        private readonly string clientId;
        private readonly TimeSpan timeout;
        private volatile int lastCorrelationId;

        /// <summary>
        /// Initializes a new instance of the <see cref="BrokerConnection"/> class.
        /// </summary>
        /// <param name="address">The broker address</param>
        /// <param name="clientId">The client identifier</param>
        /// <param name="timeout">The interval of time to wait for a operation to be completed. The default value is 5 seconds.</param>
        public BrokerConnection(BrokerAddress address, string clientId, TimeSpan? timeout)
        {
            this.clientId = clientId;
            this.timeout = timeout ?? TimeSpan.FromSeconds(10);
            this.client = new TcpClient(address.Host, address.Port);
            this.stream = this.client.GetStream();

            this.listenerTask = Task.Run(this.ListenStream);
        }

        /// <inheritdoc/>
        public Task<TResponse> SendAsync<TResponse>(IRequestMessage<TResponse> message)
            where TResponse : class, IResponse
        {
            var request = new PendingRequest(message.ResponseType);

            using var writer = new MemoryWriter();

            var correlationId = Interlocked.Increment(ref this.lastCorrelationId);

            this.waitingResponse.TryAdd(correlationId, request);
            Debug.WriteLine($"Sent CID: {correlationId}, Type {request.ResponseType.Name}");

            writer.WriteMessage(new Request(correlationId, this.clientId, message));
            writer.Position = 0;

            lock (this.stream)
            {
                writer.CopyTo(this.stream);
            }

            return request.GetTask<TResponse>(this.timeout);
        }

        /// <inheritdoc/>
        public async ValueTask DisposeAsync()
        {
            this.stopTokenSource.Cancel();
            await this.listenerTask.ConfigureAwait(false);

            this.stopTokenSource.Dispose();
            this.listenerTask.Dispose();
            await this.stream.DisposeAsync().ConfigureAwait(false);
            this.client.Dispose();
        }

        private async Task ListenStream()
        {
            while (!this.stopTokenSource.IsCancellationRequested)
            {
                try
                {
                    var messageSize = await this.ReadMessageSizeAsync().ConfigureAwait(false);

                    Debug.WriteLine($"Received message with {messageSize}b size");

                    if (messageSize <= 0)
                    {
                        continue;
                    }

                    using var memoryStream = new MemoryReader(messageSize);
                    memoryStream.ReadFrom(this.stream);
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

            if (!this.waitingResponse.TryRemove(correlationId, out var request))
            {
                Debug.WriteLine($"Received Invalid message CID: {correlationId}");
                return;
            }

            Debug.WriteLine(
                $"Received CID: {correlationId}, Type {request.ResponseType.Name} with {source.Length:N0}b");

            var response = (IResponse)Activator.CreateInstance(request.ResponseType)!;

            if (response is ITaggedFields)
            {
                _ = source.ReadTaggedFields();
            }

            response.Read(source);

            if (source.Length != source.Position)
            {
                throw new IOException("Some data was not read from response");
            }

            request.CompletionSource.TrySetResult(response);
        }

        private async Task<int> ReadMessageSizeAsync()
        {
            await this.stream
                .ReadAsync(this.messageSizeBuffer, 0, sizeof(int), this.stopTokenSource.Token)
                .ConfigureAwait(false);

            return BinaryPrimitives.ReadInt32BigEndian(this.messageSizeBuffer);
        }

        private readonly struct PendingRequest
        {
            public readonly TaskCompletionSource<IResponse> CompletionSource;

            public PendingRequest(Type responseType)
            {
                this.ResponseType = responseType;
                this.CompletionSource = new TaskCompletionSource<IResponse>();
            }

            public Type ResponseType { get; }

            public async Task<TResponse> GetTask<TResponse>(TimeSpan timeout)
                where TResponse : IResponse
            {
                var requestTask = this.CompletionSource.Task.ContinueWith(x => (TResponse)x.Result);
                var resultTask = await Task.WhenAny(requestTask, Task.Delay(timeout)).ConfigureAwait(false);

                if (resultTask != requestTask)
                {
                    throw new TimeoutException($"The operation {this.ResponseType.FullName} has timed out");
                }

                return requestTask.GetAwaiter().GetResult();
            }
        }
    }
}
