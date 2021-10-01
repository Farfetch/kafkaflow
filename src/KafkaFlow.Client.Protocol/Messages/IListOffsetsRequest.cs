namespace KafkaFlow.Client.Protocol.Messages
{
    public interface IListOffsetsRequest : IRequestMessage<IListOffsetsResponse>
    {
        int ReplicaId { get; }

        byte IsolationLevel { get; }

        ITopic[] Topics { get; }

        public interface ITopic : IRequest
        {
            string Name { get; }

            IPartition[] Partitions { get; }
        }

        public interface IPartition : IRequest
        {
            int PartitionIndex { get; }

            int CurrentLeaderEpoch { get; }

            long Timestamp { get; }
        }
    }
}
