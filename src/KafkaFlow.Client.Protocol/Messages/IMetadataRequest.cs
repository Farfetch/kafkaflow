namespace KafkaFlow.Client.Protocol.Messages
{
    /// <summary>
    ///  Used to create Metadata requests
    /// </summary>
    public interface IMetadataRequest : IRequestMessage<IMetadataResponse>
    {
        /// <summary>
        /// Gets a value indicating whether the broker may auto-create topics or not
        /// </summary>
        bool AllowAutoTopicCreation { get; }

        /// <summary>
        /// Gets a value indicating whether to include cluster authorized operations or not
        /// </summary>
        bool IncludeClusterAuthorizedOperations { get; }

        /// <summary>
        /// Gets a value indicating whether to include topic authorized operations or not
        /// </summary>
        bool IncludeTopicAuthorizedOperations { get; }

        /// <summary>
        /// Adds a topic to the request
        /// </summary>
        /// <param name="topicName"></param>
        void AddTopic(string topicName);

        /// <summary>
        /// Used to create a topic entity
        /// </summary>
        public interface ITopic : IRequest
        {
            /// <summary>
            /// Gets the topic name
            /// </summary>
            string Name { get; }
        }
    }
}
