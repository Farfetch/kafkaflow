namespace KafkaFlow.Client.Producers
{
    using System.Threading.Tasks;
    using KafkaFlow.Client.Protocol.Messages;

    internal class ProduceQueueItem
    {
        public ProduceData Data { get; }

        public int PartitionId { get; }

        public TaskCompletionSource<ProduceResult> CompletionSource { get; }

        public int OffsetDelta { get; set; }

        public ProduceQueueItem(
            ProduceData data,
            int partitionId,
            TaskCompletionSource<ProduceResult> completionSource)
        {
            this.Data = data;
            this.PartitionId = partitionId;
            this.CompletionSource = completionSource;
        }
    }
}
