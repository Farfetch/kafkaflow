namespace KafkaFlow.Client.Producers
{
    using System;
    using System.Threading.Tasks;
    using KafkaFlow.Client.Protocol.Messages;

    public class ProduceItem
    {
        private readonly TaskCompletionSource<ProduceItem> completionSource = new();

        public ProduceItem(
            string topicName,
            Memory<byte> key,
            Memory<byte> value,
            Headers headers,
            int partitionId)
        {
            this.TopicName = topicName;
            this.Key = key;
            this.Value = value;
            this.Headers = headers;
            this.PartitionId = partitionId;
            this.Offset = -1;
        }

        public string TopicName { get; }

        public Memory<byte> Key { get; }

        public Memory<byte> Value { get; }

        public Headers Headers { get; }

        public int PartitionId { get; }

        public int OffsetDelta { get; set; }

        public long Offset { get; private set; }

        public Task<ProduceItem> CompletionTask => this.completionSource.Task;

        public void SetResult(long baseOffset)
        {
            this.Offset = this.OffsetDelta + baseOffset;
            this.completionSource.SetResult(this);
        }

        public void SetException(Exception ex)
        {
            this.completionSource.SetException(ex);
        }
    }
}
