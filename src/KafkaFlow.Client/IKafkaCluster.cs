namespace KafkaFlow.Client
{
    using System;
    using System.Collections.Generic;
    using KafkaFlow.Client.Metadata;

    public interface IKafkaCluster : IAsyncDisposable
    {
        IReadOnlyDictionary<int, IKafkaBroker> Brokers { get; }

        IMetadataClient MetadataClient { get; }
    }
}
