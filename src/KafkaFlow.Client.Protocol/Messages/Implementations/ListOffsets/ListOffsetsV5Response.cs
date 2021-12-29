namespace KafkaFlow.Client.Protocol.Messages.Implementations.ListOffsets
{
    using KafkaFlow.Client.Protocol.Streams;

    public class ListOffsetsV5Response : IListOffsetsResponse
    {
        public int ThrottleTimeMs { get; set; }

        public IListOffsetsResponse.ITopic[] Topics { get; set; }

        public void Read(MemoryReader source)
        {
            this.ThrottleTimeMs = source.ReadInt32();
            this.Topics = source.ReadArray<Topic>();
        }

        public class Topic : IListOffsetsResponse.ITopic
        {
            public string Name { get; private set; }

            public IListOffsetsResponse.IPartition[] Partitions { get; private set; }

            public void Read(MemoryReader source)
            {
                this.Name = source.ReadString();
                this.Partitions = source.ReadArray<Partition>();
            }
        }

        public class Partition : IListOffsetsResponse.IPartition
        {
            public int PartitionIndex { get; private set; }

            public ErrorCode ErrorCode { get; private set; }

            public long Timestamp { get; private set; }

            public long Offset { get; private set; }

            public int LeaderEpoch { get; private set; }

            public void Read(MemoryReader source)
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
