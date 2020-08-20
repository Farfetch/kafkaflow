namespace KafkaFlow.Client.Protocol.Messages.Implementations.Metadata
{
    using System;
    using KafkaFlow.Client.Protocol.Streams;

    /// <summary>
    /// Response message with the metadata of a topics list
    /// </summary>
    internal class MetadataV9Response : IMetadataResponse, ITaggedFields
    {
        /// <inheritdoc/>
        public int ThrottleTime { get; private set; }

        /// <inheritdoc/>
        public IMetadataResponse.IBroker[] Brokers { get; private set; } = Array.Empty<IMetadataResponse.IBroker>();

        /// <inheritdoc/>
        public string? ClusterId { get; private set; }

        /// <inheritdoc/>
        public int ControllerId { get; private set; }

        /// <inheritdoc/>
        public IMetadataResponse.ITopic[] Topics { get; private set; } = Array.Empty<IMetadataResponse.ITopic>();

        /// <inheritdoc/>
        public int ClusterAuthorizedOperations { get; private set; }

        /// <inheritdoc/>
        public TaggedField[] TaggedFields { get; private set; } = Array.Empty<TaggedField>();

        /// <inheritdoc/>
        void IResponse.Read(MemoryReader source)
        {
            this.ThrottleTime = source.ReadInt32();
            this.Brokers = source.ReadCompactArray<Broker>();
            this.ClusterId = source.ReadCompactNullableString();
            this.ControllerId = source.ReadInt32();
            this.Topics = source.ReadCompactArray<Topic>();
            this.ClusterAuthorizedOperations = source.ReadInt32();
            this.TaggedFields = source.ReadTaggedFields();
        }

        private class Topic : IMetadataResponse.ITopic, ITaggedFields
        {
            public ErrorCode Error { get; private set; }

            public string Name { get; private set; } = string.Empty;

            public bool IsInternal { get; private set; }

            public IMetadataResponse.IPartition[] Partitions { get; private set; } = Array.Empty<IMetadataResponse.IPartition>();

            public int TopicAuthorizedOperations { get; private set; }

            public TaggedField[] TaggedFields { get; private set; } = Array.Empty<TaggedField>();

            void IResponse.Read(MemoryReader source)
            {
                this.Error = source.ReadErrorCode();
                this.Name = source.ReadCompactString();
                this.IsInternal = source.ReadBoolean();
                this.Partitions = source.ReadCompactArray<Partition>();
                this.TopicAuthorizedOperations = source.ReadInt32();
                this.TaggedFields = source.ReadTaggedFields();
            }
        }

        private class Partition : IMetadataResponse.IPartition, ITaggedFields
        {
            public ErrorCode Error { get; private set; }

            public int Id { get; private set; }

            public int LeaderId { get; private set; }

            public int LeaderEpoch { get; private set; }

            public int[] ReplicaNodes { get; private set; } = Array.Empty<int>();

            public int[] IsrNodes { get; private set; } = Array.Empty<int>();

            public int[] OfflineReplicas { get; private set; } = Array.Empty<int>();

            public TaggedField[] TaggedFields { get; private set; } = Array.Empty<TaggedField>();

            void IResponse.Read(MemoryReader source)
            {
                this.Error = source.ReadErrorCode();
                this.Id = source.ReadInt32();
                this.LeaderId = source.ReadInt32();
                this.LeaderEpoch = source.ReadInt32();
                this.ReplicaNodes = source.ReadCompactInt32Array();
                this.IsrNodes = source.ReadCompactInt32Array();
                this.OfflineReplicas = source.ReadCompactInt32Array();
                this.TaggedFields = source.ReadTaggedFields();
            }
        }

        private class Broker : IMetadataResponse.IBroker, ITaggedFields
        {
            public int NodeId { get; private set; }

            public string Host { get; private set; } = string.Empty;

            public int Port { get; private set; }

            public string? Rack { get; private set; }

            public TaggedField[] TaggedFields { get; private set; } = Array.Empty<TaggedField>();

            void IResponse.Read(MemoryReader source)
            {
                this.NodeId = source.ReadInt32();
                this.Host = source.ReadCompactString();
                this.Port = source.ReadInt32();
                this.Rack = source.ReadCompactNullableString();
                this.TaggedFields = source.ReadTaggedFields();
            }
        }
    }
}
