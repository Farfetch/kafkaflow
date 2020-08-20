namespace KafkaFlow.Client.Protocol.Messages
{
    using System.IO;

    public class OffsetCommitV2Request : IRequestMessage<OffsetCommitV2Response>
    {
        public OffsetCommitV2Request(
            string groupId,
            int groupGenerationId,
            string memberId,
            long retentionTimeMs,
            Topic[] topics)
        {
            this.GroupId = groupId;
            this.GroupGenerationId = groupGenerationId;
            this.MemberId = memberId;
            this.RetentionTimeMs = retentionTimeMs;
            this.Topics = topics;
        }

        public string GroupId { get; }

        public int GroupGenerationId { get; }

        public string MemberId { get; }

        public long RetentionTimeMs { get; }

        public Topic[] Topics { get; }

        public ApiKey ApiKey => ApiKey.OffsetCommit;

        public short ApiVersion => 2;

        public void Write(Stream destination)
        {
            destination.WriteString(this.GroupId);
            destination.WriteInt32(this.GroupGenerationId);
            destination.WriteInt64(this.RetentionTimeMs);
            destination.WriteArray(this.Topics);
        }

        public class Topic : IRequest
        {
            public Topic(string name, Partition[] partitions)
            {
                this.Name = name;
                this.Partitions = partitions;
            }

            public string Name { get; }

            public Partition[] Partitions { get; }

            public void Write(Stream destination)
            {
                destination.WriteString(this.Name);
                destination.WriteArray(this.Partitions);
            }
        }

        public class Partition : IRequest
        {
            public int Id { get; }

            public long Offset { get; }

            public string Metadata { get; }

            public void Write(Stream destination)
            {
                destination.WriteInt32(this.Id);
                destination.WriteInt64(this.Offset);
                destination.WriteString(this.Metadata);
            }
        }
    }
}
