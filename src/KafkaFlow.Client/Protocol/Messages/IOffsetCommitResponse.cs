namespace KafkaFlow.Client.Protocol.Messages.Implementations
{
    public interface IOffsetCommitResponse : IResponse
    {
        ITopic[] Topics { get; }

        public interface ITopic : IResponse
        {
            string Name { get; }

            IPartition[] Partitions { get; }
        }

        public interface IPartition : IResponse
        {
            int Id { get; }

            ErrorCode Error { get; }
        }
    }
}
