namespace KafkaFlow.Client.Protocol.Messages
{
    public interface IOffsetFetchResponse : IResponse
    {
        int ThrottleTimeMs { get; }

        ITopic[] Topics { get; }

        ErrorCode Error { get; }

        public interface ITopic : IResponse
        {
            string Name { get; }

            IPartition[] Partitions { get; }
        }

        public interface IPartition : IResponse
        {
            int Id { get; }

            long CommittedOffset { get; }

            int CommittedLeaderEpoch { get; }

            string Metadata { get; }

            ErrorCode ErrorCode { get; }
        }
    }
}
