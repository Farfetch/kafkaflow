namespace KafkaFlow.Client.Producers
{
    using System.Threading.Tasks;

    internal class ProduceQueueItem
    {
        public ProduceData Data { get; }
        public int PartitionId { get; }
        public TaskCompletionSource<ProduceResult> CompletionSource { get; }

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
