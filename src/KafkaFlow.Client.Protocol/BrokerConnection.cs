namespace KafkaFlow.Client.Protocol
{
    using System;
    using System.Buffers.Binary;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.Net.Sockets;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using KafkaFlow.Client.Protocol.Messages;
    using KafkaFlow.Client.Protocol.Streams;

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

        public BrokerConnection(BrokerAddress address, string clientId, TimeSpan timeout)
        {
            this.Address = address;
            this.clientId = clientId;
            this.timeout = timeout;
            this.client = new TcpClient(address.Host, address.Port);
            this.stream = this.client.GetStream();

            this.listenerTask = Task.Run(this.ListenStream);
        }

        public BrokerAddress Address { get; }

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

            Debug.WriteLine($"Received CID: {correlationId}, Type {request.ResponseType.Name} with {source.Length:N0}b");

            var response = (IResponse) Activator.CreateInstance(request.ResponseType)!;

            if (response is ITaggedFields)
            {
                _ = source.ReadTaggedFields();
            }

            response.Read(source);

            if (source.Length != source.Position)
            {
                throw new Exception("Some data was not read from response");
            }

            request.CompletionSource.TrySetResult(response);
        }

        private async Task<int> WaitForMessageSizeAsync()
        {
            await this.stream
                .ReadAsync(this.messageSizeBuffer, 0, sizeof(int), this.stopTokenSource.Token)
                .ConfigureAwait(false);

            return BinaryPrimitives.ReadInt32BigEndian(this.messageSizeBuffer);
        }

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

        public async ValueTask DisposeAsync()
        {
            this.stopTokenSource.Cancel();
            await this.listenerTask.ConfigureAwait(false);

            this.stopTokenSource.Dispose();
            this.listenerTask.Dispose();
            await this.stream.DisposeAsync().ConfigureAwait(false);
            this.client.Dispose();
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
                var resultTask = await Task.WhenAny(requestTask, Task.Delay(timeout));

                if (resultTask != requestTask)
                {
                    throw new TimeoutException($"The operation {this.ResponseType} has timeout");
                }

                return requestTask.GetAwaiter().GetResult();
            }
        }
    }
}
