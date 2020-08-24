namespace KafkaFlow.Client.Protocol
{
    using System;
    using System.Buffers.Binary;
    using System.Collections.Concurrent;
    using System.Net.Sockets;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

    public class KafkaHostConnection : IKafkaHostConnection
    {
        private readonly TcpClient client;
        private readonly NetworkStream stream;

        private readonly ConcurrentDictionary<int, PendingRequest> pendingRequests = new ConcurrentDictionary<int, PendingRequest>();

        private volatile int lastCorrelationId;

        private readonly CancellationTokenSource stopTokenSource = new CancellationTokenSource();
        private readonly Task listenerTask;

        private readonly byte[] messageSizeBuffer = new byte[sizeof(int)];

        private readonly string clientId;
        private readonly TimeSpan requestTimeout;

        public KafkaHostConnection(string host, int port, string clientId, TimeSpan requestTimeout)
        {
            this.clientId = clientId;
            this.requestTimeout = requestTimeout;
            this.client = new TcpClient(host, port);
            this.stream = this.client.GetStream();

            this.listenerTask = Task.Run(this.ListenStream);
        }

        private async Task ListenStream()
        {
            while (!this.stopTokenSource.IsCancellationRequested)
            {
                try
                {
                    var messageSize = await this.WaitForMessageSizeAsync().ConfigureAwait(false);

                    if (messageSize <= 0)
                        continue;

                    using var tracked = new TrackedStream(this.stream, messageSize);
                    this.RespondMessage(tracked);
                }
                catch (OperationCanceledException)
                {
                    return;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RespondMessage(TrackedStream source)
        {
            var correlationId = source.ReadInt32();

            if (!this.pendingRequests.TryRemove(correlationId, out var request))
            {
                source.DiscardRemainingData();
                return;
            }

            var message = (IResponse) Activator.CreateInstance(request.ResponseType)!;

            if (message is IResponseV2)
                _ = source.ReadTaggedFields();

            message.Read(source);

            if (source.Size != source.Position)
                throw new Exception("Some data was not read from response");

            request.CompletionSource.TrySetResult(message);
        }

        private async Task<int> WaitForMessageSizeAsync()
        {
            await this.stream
                .ReadAsync(this.messageSizeBuffer, 0, sizeof(int), this.stopTokenSource.Token)
                .ConfigureAwait(false);

            return BinaryPrimitives.ReadInt32BigEndian(this.messageSizeBuffer);
        }

        public Task<TResponse> SendAsync<TResponse>(IRequestMessage<TResponse> request)
            where TResponse : IResponse, new()
        {
            var pendingRequest = new PendingRequest(this.requestTimeout, typeof(TResponse));

            lock (this.stream)
            {
                this.pendingRequests.TryAdd(
                    Interlocked.Increment(ref this.lastCorrelationId),
                    pendingRequest);

                this.stream.WriteMessage(
                    new Request(
                        this.lastCorrelationId,
                        this.clientId,
                        request));
            }

            return pendingRequest.GetTask<TResponse>();
        }

        public void Dispose()
        {
            this.stopTokenSource.Cancel();
            this.listenerTask.GetAwaiter().GetResult();
            this.listenerTask.Dispose();
            this.stream.Dispose();
            this.client.Dispose();
        }

        private class PendingRequest
        {
            public TimeSpan Timeout { get; }

            public Type ResponseType { get; }

            public readonly TaskCompletionSource<IResponse> CompletionSource =
                new TaskCompletionSource<IResponse>();

            public PendingRequest(TimeSpan timeout, Type responseType)
            {
                this.Timeout = timeout;
                this.ResponseType = responseType;
            }

            public Task<TResponse> GetTask<TResponse>() where TResponse : IResponse =>
                this.CompletionSource.Task.ContinueWith(x => (TResponse) x.Result);
        }
    }
}
