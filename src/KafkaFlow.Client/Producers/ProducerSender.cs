namespace KafkaFlow.Client.Producers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using KafkaFlow.Client.Messages;
    using KafkaFlow.Client.Protocol.Messages;

    internal class ProducerSender : IDisposable, IAsyncDisposable
    {
        private readonly IKafkaHost host;
        private readonly ProducerConfiguration configuration;

        private ProduceRequest request;
        private volatile int messageCount;
        private DateTime lastProductionTime = DateTime.MinValue;

        private readonly Task produceTimeoutTask;
        private readonly CancellationTokenSource stopLingerProduceTokenSource = new CancellationTokenSource();
        private readonly SemaphoreSlim produceSemaphore = new SemaphoreSlim(1, 1);

        public ProducerSender(IKafkaHost host, ProducerConfiguration configuration)
        {
            this.host = host;
            this.configuration = configuration;
            this.request = new ProduceRequest(this.configuration.Acks, this.configuration.ProduceTimeout.Milliseconds);

            this.produceTimeoutTask = Task.Run(this.LingerProduceAsync);
        }

        public ValueTask EnqueueAsync(ProduceQueueItem item)
        {
            lock (this.request)
            {
                var topic = this.request.Topics.GetOrAdd(
                    item.Data.Topic,
                    _ => new ProduceRequest.Topic(item.Data.Topic));

                var partition = topic.Partitions.GetOrAdd(
                    item.PartitionId,
                    _ => new ProduceRequest.Partition(item.PartitionId, new RecordBatch()));

                partition.Batch.AddRecord(
                    new RecordBatch.Record
                    {
                        Key = item.Data.Key,
                        Value = item.Data.Value,
                        // TODO: copy headers
                        //Headers = 
                    });
            }

            return Interlocked.Increment(ref this.messageCount) < this.configuration.MaxProduceBatchSize ?
                default :
                new ValueTask(this.ProduceAsync());
        }

        private async Task ProduceAsync()
        {
            try
            {
                if (this.messageCount == 0)
                    return;

                await this.produceSemaphore.WaitAsync().ConfigureAwait(false);

                try
                {
                    if (Interlocked.Exchange(ref this.messageCount, 0) == 0)
                        return;

                    var queued = Interlocked.Exchange(
                        ref this.request,
                        new ProduceRequest(this.configuration.Acks, this.configuration.ProduceTimeout.Milliseconds));

                    var result = await this.host.SendAsync(queued).ConfigureAwait(false);
                }
                finally
                {
                    this.produceSemaphore.Release();
                }
            }
            catch
            {
                // TODO: some kind of log or retry on errors
            }
            finally
            {
                this.lastProductionTime = DateTime.Now;
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
