namespace KafkaFlow.Client.Messages
{
    using System;
    using System.Collections.Generic;
    using KafkaFlow.Client.Protocol;

    internal class MetadataRequest : IClientRequest<MetadataResponse>
    {
        public MetadataRequest(IReadOnlyCollection<string> topics)
        {
            this.Topics = topics;
        }

        public MetadataRequest() : this(Array.Empty<string>())
        {
        }

        public ApiKey ApiKey => ApiKey.Metadata;

        public IReadOnlyCollection<string> Topics { get; }
    }
}
