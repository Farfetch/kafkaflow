namespace KafkaFlow.Client.Protocol.Messages.Implementations.OffsetFetch
{
    using KafkaFlow.Client.Protocol.Streams;

    public class OffsetFetchV5Response : IOffsetFetchResponse
    {
        public int ThrottleTimeMs { get; set; }

        public IOffsetFetchResponse.ITopic[] Topics { get; set; }

        public ErrorCode Error { get; set; }

        public void Read(MemoryReader source)
        {
            this.ThrottleTimeMs = source.ReadInt32();
            this.Topics = source.ReadArray<Topic>();
            this.Error = source.ReadErrorCode();
        }

        public class Topic : IOffsetFetchResponse.ITopic
        {
            public string Name { get; set; }

            public IOffsetFetchResponse.IPartition[] Partitions { get; set; }

            public void Read(MemoryReader source)
            {
                this.Name = source.ReadString();
                this.Partitions = source.ReadArray<Partition>();
            }
        }

        public class Partition : IOffsetFetchResponse.IPartition
        {
            public int Id { get; set; }

            public long CommittedOffset { get; set; }

            public int CommittedLeaderEpoch { get; set; }

            public string Metadata { get; set; }

            public ErrorCode ErrorCode { get; set; }

            public void Read(MemoryReader source)
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
