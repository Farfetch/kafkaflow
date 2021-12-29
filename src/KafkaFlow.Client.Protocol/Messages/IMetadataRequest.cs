namespace KafkaFlow.Client.Protocol.Messages
{
    public interface IMetadataRequest : IRequestMessage<IMetadataResponse>
    {
        bool AllowAutoTopicCreation { get; set; }

        bool IncludeClusterAuthorizedOperations { get; set; }

        bool IncludeTopicAuthorizedOperations { get; set; }

        void AddTopic(string topicName);

        public interface ITopic : IRequest
        {
            string Name { get; set; }
        }
    }
}
