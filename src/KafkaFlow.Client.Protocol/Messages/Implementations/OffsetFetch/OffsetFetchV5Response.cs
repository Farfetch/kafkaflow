namespace KafkaFlow.Client.Protocol.Messages.Implementations.OffsetFetch
{
    using KafkaFlow.Client.Protocol.Streams;

    /// <summary>
    /// Response message with the list of committed offsets in a topic by a consumer group
    /// </summary>
    public class OffsetFetchV5Response : IOffsetFetchResponse
    {
        /// <inheritdoc/>
        public int ThrottleTimeMs { get; private set; }

        /// <inheritdoc/>
        public IOffsetFetchResponse.ITopic[] Topics { get; private set; }

        /// <inheritdoc/>
        public ErrorCode Error { get; private set; }

        /// <inheritdoc/>
        void IResponse.Read(MemoryReader source)
        {
            this.ThrottleTimeMs = source.ReadInt32();
            this.Topics = source.ReadArray<Topic>();
            this.Error = source.ReadErrorCode();
        }

        private class Topic : IOffsetFetchResponse.ITopic
        {
            public string Name { get; private set; }

            public IOffsetFetchResponse.IPartition[] Partitions { get; private set; }

            void IResponse.Read(MemoryReader source)
            {
                this.Name = source.ReadString();
                this.Partitions = source.ReadArray<Partition>();
            }
        }

        private class Partition : IOffsetFetchResponse.IPartition
        {
            public int Id { get; private set; }

            public long CommittedOffset { get; private set; }

            public int CommittedLeaderEpoch { get; private set; }

            public string Metadata { get; private set; }

            public ErrorCode ErrorCode { get; private set; }

            void IResponse.Read(MemoryReader source)
            {
                this.Id = source.ReadInt32();
                this.CommittedOffset = source.ReadInt64();
                this.CommittedLeaderEpoch = source.ReadInt32();
                this.Metadata = source.ReadString();
                this.ErrorCode = source.ReadErrorCode();
            }
        }
    }
}
