namespace KafkaFlow.Client.Exceptions
{
    using System;
    using KafkaFlow.Client.Protocol.Messages;

    public class ProduceException : Exception
    {
        public ErrorCode ErrorCode { get; }
        public string? PartitionMessage { get; }
        public string? RecordMessage { get; }

        public ProduceException(ErrorCode errorCode, string? partitionMessage, string? recordMessage)
        {
            this.ErrorCode = errorCode;
            this.PartitionMessage = partitionMessage;
            this.RecordMessage = recordMessage;
        }
    }
}
