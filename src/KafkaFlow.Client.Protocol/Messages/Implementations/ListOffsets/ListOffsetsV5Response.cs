namespace KafkaFlow.Client.Protocol.Messages.Implementations.ListOffsets
{
    using KafkaFlow.Client.Protocol.Streams;

    /// <summary>
    /// Response message with the list of committed offsets in a topic/partitions
    /// </summary>
    public class ListOffsetsV5Response : IListOffsetsResponse
    {
        /// <inheritdoc/>
        public int ThrottleTimeMs { get; private set; }

        /// <inheritdoc/>
        public IListOffsetsResponse.ITopic[] Topics { get; private set; }

        /// <inheritdoc/>
        void IResponse.Read(MemoryReader source)
        {
            this.ThrottleTimeMs = source.ReadInt32();
            this.Topics = source.ReadArray<Topic>();
        }

        /// <summary>
        /// Represents a topic
        /// </summary>
        private class Topic : IListOffsetsResponse.ITopic
        {
            /// <inheritdoc/>
            public string Name { get; private set; }

            /// <inheritdoc/>
            public IListOffsetsResponse.IPartition[] Partitions { get; private set; }

            /// <inheritdoc/>
            void IResponse.Read(MemoryReader source)
            {
                this.Name = source.ReadString();
                this.Partitions = source.ReadArray<Partition>();
            }
        }

        /// <summary>
        /// Represents a partition
        /// </summary>
        private class Partition : IListOffsetsResponse.IPartition
        {
            /// <inheritdoc/>
            public int PartitionIndex { get; private set; }

            /// <inheritdoc/>
            public ErrorCode ErrorCode { get; private set; }

            /// <inheritdoc/>
            public long Timestamp { get; private set; }

            /// <inheritdoc/>
            public long Offset { get; private set; }

            /// <inheritdoc/>
            public int LeaderEpoch { get; private set; }

            /// <inheritdoc/>
            void IResponse.Read(MemoryReader source)
            {
                this.PartitionIndex = source.ReadInt32();
                this.ErrorCode = source.ReadErrorCode();
                this.Timestamp = source.ReadInt64();
                this.Offset = source.ReadInt64();
                this.LeaderEpoch = source.ReadInt32();
            }
        }
    }
}
