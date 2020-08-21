namespace KafkaFlow.Client.Messages
{
    using System.Collections.Generic;
    using KafkaFlow.Client.Protocol;

    internal class MetadataRequest : IClientRequest<MetadataResponse>
    {
        public MetadataRequest(IReadOnlyCollection<string> topics)
        {
            this.Topics = topics;
        }

        public ApiKey ApiKey => ApiKey.Metadata;

        public IReadOnlyCollection<string> Topics { get; }
    }
}
