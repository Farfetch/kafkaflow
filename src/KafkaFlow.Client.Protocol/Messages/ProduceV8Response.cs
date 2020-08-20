namespace KafkaFlow.Client.Protocol.Messages
{
    using System.IO;

    public class ProduceV8Response : IResponse
    {
        public Topic[] Topics { get; set; }

        public int ThrottleTimeMs { get; set; }

        public void Read(Stream source)
        {
            this.Topics = source.ReadArray<Topic>();
            this.ThrottleTimeMs = source.ReadInt32();
        }

        public class Topic : IResponse
        {
            public string Name { get; set; }

            public Partition[] Partitions { get; set; }

            public void Read(Stream source)
            {
                this.Name = source.ReadString();
                this.Partitions = source.ReadArray<Partition>();
            }
        }

        public class Partition : IResponse
        {
            public int Id { get; set; }

            public ErrorCode Error { get; set; }

            public long Offset { get; set; }

            public long LogAppendTime { get; set; }

            public long LogStartOffset { get; set; }

            public RecordError[] Errors { get; set; }

            public string? ErrorMessage { get; set; }

            public void Read(Stream source)
            {
                this.Id = source.ReadInt32();
                this.Error = source.ReadErrorCode();
                this.Offset = source.ReadInt64();
                this.LogAppendTime = source.ReadInt64();
                this.LogStartOffset = source.ReadInt64();
                this.Errors = source.ReadArray<RecordError>();
                this.ErrorMessage = source.ReadNullableString();
            }
        }

        public class RecordError : IResponse
        {
            public int BatchIndex { get; set; }

            public string? Message { get; set; }

            public void Read(Stream source)
            {
                this.BatchIndex = source.ReadInt32();
                this.Message = source.ReadNullableString();
            }
        }
    }
}
