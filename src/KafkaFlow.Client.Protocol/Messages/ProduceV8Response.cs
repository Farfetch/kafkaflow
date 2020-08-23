namespace KafkaFlow.Client.Protocol.Messages
{
    using System.IO;

    public class ProduceV8Response : IResponse
    {
        public Topic[] Topics { get; private set; }

        public int ThrottleTimeMs { get; private set; }

        public void Read(Stream source)
        {
            this.Topics = source.ReadArray<Topic>();
            this.ThrottleTimeMs = source.ReadInt32();
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

            public long BaseOffset { get; private set; }

            public long LogAppendTime { get; private set; }

            public long LogStartOffset { get; private set; }

            public RecordError[] Errors { get; private set; }

            public string? ErrorMessage { get; private set; }

            public void Read(Stream source)
            {
                this.Id = source.ReadInt32();
                this.Error = source.ReadErrorCode();
                this.BaseOffset = source.ReadInt64();
                this.LogAppendTime = source.ReadInt64();
                this.LogStartOffset = source.ReadInt64();
                this.Errors = source.ReadArray<RecordError>();
                this.ErrorMessage = source.ReadNullableString();
            }
        }

        public class RecordError : IResponse
        {
            public int BatchIndex { get; private set; }

            public string? Message { get; private set; }

            public void Read(Stream source)
            {
                this.BatchIndex = source.ReadInt32();
                this.Message = source.ReadNullableString();
            }
        }
    }
}
