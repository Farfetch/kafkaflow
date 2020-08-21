namespace KafkaFlow.Client.Messages
{
    using KafkaFlow.Client.Protocol.Messages;

    internal class ProduceResponse : IClientResponse
    {
        public ProduceResponse(Topic[] topics)
        {
            this.Topics = topics;
        }

        public Topic[] Topics { get; }

        public class Topic
        {
            public Topic(string name, Partition[] partitions)
            {
                this.Name = name;
                this.Partitions = partitions;
            }

            public string Name { get; }

            public Partition[] Partitions { get; }
        }

        public class Partition
        {
            public Partition(
                int id,
                ErrorCode error,
                long offset,
                RecordError[] errors,
                string? errorMessage)
            {
                this.Id = id;
                this.Error = error;
                this.Offset = offset;
                this.Errors = errors;
                this.ErrorMessage = errorMessage;
            }

            public int Id { get; }

            public ErrorCode Error { get; }

            public long Offset { get; }

            public RecordError[] Errors { get; }

            public string? ErrorMessage { get; }
        }

        public class RecordError
        {
            public RecordError(int batchIndex, string? message)
            {
                this.BatchIndex = batchIndex;
                this.Message = message;
            }

            public int BatchIndex { get; }

            public string? Message { get; }
        }
    }
}
