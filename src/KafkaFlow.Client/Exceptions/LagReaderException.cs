namespace KafkaFlow.Client.Exceptions
{
    using System;
    using System.Collections.Generic;
    using KafkaFlow.Client.Extensions;
    using KafkaFlow.Client.Protocol.Messages;

    /// <summary>
    /// Represents an error trying to get the kafka metrics
    /// </summary>
    public class LagReaderException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LagReaderException"/> class.
        /// </summary>
        /// <param name="errorCode">The error code to associate with this exception</param>
        /// <param name="topicName">The topic name associate with this exception</param>
        /// <param name="partitions">The partitions associate with this exception</param>
        /// <param name="groupName">The group name associate with this exception</param>
        public LagReaderException(
            ErrorCode errorCode,
            string topicName,
            IEnumerable<int>? partitions = null,
            string? groupName = null)
            : base(errorCode.GetDescription())
        {
            this.ErrorCode = errorCode;
            this.TopicName = topicName;
            this.Partitions = partitions;
            this.GroupName = groupName;
        }

        /// <summary>
        /// Gets the error code for this exception
        /// </summary>
        public ErrorCode ErrorCode { get; }

        /// <summary>
        /// Gets the topic name for this exception
        /// </summary>
        public string TopicName { get; }

        /// <summary>
        /// Gets the partitions for this exception
        /// </summary>
        public IEnumerable<int>? Partitions { get; }

        /// <summary>
        /// Gets the group name for this exception
        /// </summary>
        public string? GroupName { get; }
    }
}
