namespace KafkaFlow.Client.Protocol.Messages
{
    using KafkaFlow.Client.Protocol.Messages.Implementations;

    public interface IMetadataResponse : IResponse
    {
        int ThrottleTime { get; }

        IBroker[] Brokers { get; }

        string? ClusterId { get; }

        int ControllerId { get; }

        ITopic[] Topics { get; }

        int ClusterAuthorizedOperations { get; }

        public interface ITopic : IResponse
        {
            ErrorCode Error { get; }

            string Name { get; }

            bool IsInternal { get; }

            IPartition[] Partitions { get; }

            int TopicAuthorizedOperations { get; }
        }

        public interface IPartition : IResponse
        {
            ErrorCode Error { get; }

            int Id { get; }

            int LeaderId { get; }

            int LeaderEpoch { get; }

            int[] ReplicaNodes { get; }

            int[] IsrNodes { get; }

            int[] OfflineReplicas { get; }
        }

        public interface IBroker : IResponse
        {
            int NodeId { get; }

            string Host { get; }

            int Port { get; }

            string? Rack { get; }
        }
    }
}
