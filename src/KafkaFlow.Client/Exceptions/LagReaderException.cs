namespace KafkaFlow.Client.Exceptions
{
    using System;
    using System.Collections.Generic;
    using KafkaFlow.Client.Extensions;
    using KafkaFlow.Client.Protocol.Messages;

    public class LagReaderException : Exception
    {
        public ErrorCode ErrorCode { get; }

        public string TopicName { get; }

        public IEnumerable<int> Partitions { get; }

        public string? GroupName { get; }

        public LagReaderException(
            ErrorCode errorCode,
            string topicName,
            IEnumerable<int> partitions,
            string groupName = null!)
            : base(errorCode.GetDescription())
        {
            this.ErrorCode = errorCode;
            this.TopicName = topicName;
            this.Partitions = partitions;
            this.GroupName = groupName;
        }
    }
}
