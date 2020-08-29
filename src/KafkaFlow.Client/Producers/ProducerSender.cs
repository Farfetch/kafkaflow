namespace KafkaFlow.Client.Producers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using KafkaFlow.Client.Exceptions;
    using KafkaFlow.Client.Extensions;
    using KafkaFlow.Client.Protocol.Messages;
    using KafkaFlow.Client.Protocol.Messages.Implementations;

    internal class ProducerSender : IDisposable, IAsyncDisposable
    {
        private readonly IKafkaBroker broker;
        private readonly ProducerConfiguration configuration;

        private IProduceRequest request;
        private volatile int messageCount;
        private DateTime lastProductionTime = DateTime.MinValue;

        private readonly Task produceTimeoutTask;
        private readonly CancellationTokenSource stopLingerProduceTokenSource = new CancellationTokenSource();
        private readonly SemaphoreSlim produceSemaphore = new SemaphoreSlim(1, 1);

        private SortedDictionary<(string, int), LinkedList<ProduceQueueItem>> pendingRequests
            = new SortedDictionary<(string, int), LinkedList<ProduceQueueItem>>();

        public ProducerSender(
            IKafkaBroker broker,
            ProducerConfiguration configuration)
        {
            this.broker = broker;
            this.configuration = configuration;
            this.request = broker.RequestFactory.CreateProduce(
                this.configuration.Acks,
                this.configuration.ProduceTimeout.Milliseconds);

            this.produceTimeoutTask = Task.Run(this.LingerProduceAsync);
        }

        public async ValueTask EnqueueAsync(ProduceQueueItem item)
        {
            await this.produceSemaphore.WaitAsync().ConfigureAwait(false);

            try
            {
                var topic = this.request.Topics.GetOrAdd(
                    item.Data.Topic,
                    _ => this.request.CreateTopic(item.Data.Topic));

                var partition = topic.Partitions.GetOrAdd(
                    item.PartitionId,
                    _ => topic.CreatePartition(item.PartitionId));

                partition.RecordBatch.AddRecord(
                    new RecordBatch.Record
                    {
                        Key = item.Data.Key,
                        Value = item.Data.Value,
                        // TODO: copy headers
                        //Headers = 
                    });

                item.OffsetDelta = partition.RecordBatch.LastOffsetDelta;

                this.pendingRequests
                    .SafeGetOrAdd(
                        (item.Data.Topic, item.PartitionId),
                        key => new LinkedList<ProduceQueueItem>())
                    .AddLast(item);
            }
            finally
            {
                this.produceSemaphore.Release();
            }

            if (Interlocked.Increment(ref this.messageCount) >= this.configuration.MaxProduceBatchSize)
                await this.ProduceAsync().ConfigureAwait(false);
        }

        private async Task ProduceAsync()
        {
            try
            {
                if (this.messageCount == 0)
                    return;

                Task<IProduceResponse> resultTask;
                SortedDictionary<(string, int), LinkedList<ProduceQueueItem>> requests;

                await this.produceSemaphore.WaitAsync().ConfigureAwait(false);

                try
                {
                    if (Interlocked.Exchange(ref this.messageCount, 0) == 0)
                        return;

                    var queued = Interlocked.Exchange(
                        ref this.request,
                        this.broker.RequestFactory.CreateProduce(
                            this.configuration.Acks,
                            this.configuration.ProduceTimeout.Milliseconds));

                    resultTask = this.broker.Connection.SendAsync(queued);

                    requests = Interlocked.Exchange(
                        ref this.pendingRequests,
                        new SortedDictionary<(string, int), LinkedList<ProduceQueueItem>>());
                }
                finally
                {
                    this.produceSemaphore.Release();
                    this.lastProductionTime = DateTime.Now;
                }

                this.RespondRequests(
                    await resultTask.ConfigureAwait(false),
                    requests);
            }
            catch (Exception e)
            {
                // TODO: some kind of log or retry on errors
            }
        }

        private void RespondRequests(
            IProduceResponse result,
            IDictionary<(string, int), LinkedList<ProduceQueueItem>> requests)
        {
            foreach (var topic in result.Topics)
            {
                foreach (var partition in topic.Partitions)
                {
                    if (!requests.TryGetValue((topic.Name, partition.Id), out var items))
                        continue;

                    requests.Remove((topic.Name, partition.Id));

                    foreach (var item in items)
                    {
                        if (
                            partition.Error == ErrorCode.None &&
                            partition.RecordErrors.All(x => x.BatchIndex != item.OffsetDelta))
                        {
                            item.CompletionSource.SetResult(
                                new ProduceResult(
                                    topic.Name,
                                    partition.Id,
                                    partition.BaseOffset + item.OffsetDelta,
                                    item.Data));
                        }
                        else
                        {
                            var recordError = partition.RecordErrors
                                .FirstOrDefault(x => x.BatchIndex == item.OffsetDelta);

                            item.CompletionSource.SetException(
                                new ProduceException(
                                    partition.Error,
                                    partition.ErrorMessage,
                                    recordError?.Message));
                        }
                    }
                }
            }
        }

        private async Task LingerProduceAsync()
        {
            try
            {
                while (!this.stopLingerProduceTokenSource.IsCancellationRequested)
                {
                    var diff = DateTime.Now - this.lastProductionTime;
                    if (diff < this.configuration.Linger)
                    {
                        await Task
                            .Delay(
                                this.configuration.Linger - diff,
                                this.stopLingerProduceTokenSource.Token)
                            .ConfigureAwait(false);

                        continue;
                    }

                    await this.ProduceAsync().ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException)
            {
                // Do nothing
            }

            await this.ProduceAsync().ConfigureAwait(false);
        }

        public void Dispose()
        {
            this.DisposeAsync().GetAwaiter().GetResult();
        }

        public async ValueTask DisposeAsync()
        {
            this.produceSemaphore.Dispose();
            this.stopLingerProduceTokenSource.Cancel();
            await this.produceTimeoutTask.ConfigureAwait(false);
            this.produceTimeoutTask.Dispose();
        }
    }
}
