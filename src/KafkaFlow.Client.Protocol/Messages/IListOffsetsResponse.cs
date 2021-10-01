namespace KafkaFlow.Client.Protocol.Messages
{
    public interface IListOffsetsResponse : IResponse
    {
        int ThrottleTimeMs { get; }

        ITopic[] Topics { get; }

        public interface ITopic : IResponse
        {
            string Name { get; }

            IPartition[] Partitions { get; }
        }

        public interface IPartition : IResponse
        {
            int PartitionIndex { get; }

            ErrorCode ErrorCode { get; }

            long Timestamp { get; }

            long Offset { get; }

            int LeaderEpoch { get; }
        }
    }
}
