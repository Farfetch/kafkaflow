namespace KafkaFlow.Client.Producers
{
    using System.Threading;
    using System.Threading.Tasks;
    using KafkaFlow.Client.Messages;
    using KafkaFlow.Client.Protocol.Messages;

    internal class ProducerSender
    {
        private readonly IKafkaHost host;
        private readonly ProducerConfiguration configuration;

        private ProduceRequest request;
        private int messageCount;

        public ProducerSender(IKafkaHost host, ProducerConfiguration configuration)
        {
            this.host = host;
            this.configuration = configuration;
            this.request = new ProduceRequest(this.configuration.Acks, this.configuration.Timeout.Milliseconds);
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
                this.ProduceAsync();
        }

        private ValueTask ProduceAsync()
        {
            var queued = Interlocked.Exchange(
                ref this.request,
                new ProduceRequest(this.configuration.Acks, this.configuration.Timeout.Milliseconds));

            Interlocked.Exchange(ref this.messageCount, 0);

            return new ValueTask(this.host.SendAsync(queued));
        }
    }
}
