namespace KafkaFlow.Client.Protocol.Messages
{
    public interface IMetadataRequest : IRequestMessage<IMetadataResponse>
    {
        ITopic[] Topics { get; set; }

        bool AllowAutoTopicCreation { get; set; }

        bool IncludeClusterAuthorizedOperations { get; set; }

        bool IncludeTopicAuthorizedOperations { get; set; }

        ITopic CreateTopic(string topicName);

        public interface ITopic : IRequest
        {
            string Name { get; set; }
        }
    }
}
