namespace KafkaFlow.Client.Protocol.Messages.Implementations
{
    using System.IO;

    public class OffsetCommitV2Response : IResponse
    {
        public Topic[] Topics { get; private set; }

        public void Read(Stream source)
        {
            this.Topics = source.ReadArray<Topic>();
        }

        public class Topic : IResponse
        {
            public string Name { get; private set; }

            public Partition[] Partitions { get; private set; }

            public void Read(Stream source)
            {
                this.Name = source.ReadString();
                this.Partitions = source.ReadArray<Partition>();
            }
        }

        public class Partition : IResponse
        {
            public int Id { get; private set; }

            public ErrorCode Error { get; private set; }

            public void Read(Stream source)
            {
                this.Id = source.ReadInt32();
                this.Error = source.ReadErrorCode();
            }
        }
    }
}
