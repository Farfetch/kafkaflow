namespace KafkaFlow.Admin.WebApi.Contracts
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The response of the telemetry consumer groups
    /// </summary>
    public class TelemetryResponse
    {
        public IEnumerable<ConsumerGroup> Groups { get; set; }

        /// <summary>
        /// The response of the telemetry consumer group response
        /// </summary>
        public class ConsumerGroup
        {
            /// <summary>
            /// Gets or sets the consumer group id
            /// </summary>
            public string GroupId { get; set; }

            /// <summary>
            /// Gets or sets the consumers collection
            /// </summary>
            public IEnumerable<Consumer> Consumers { get; set; }
        }

        /// <summary>
        /// The response of the telemetry consumer response
        /// </summary>
        public class Consumer
        {
            /// <summary>
            /// Gets or sets the consumer´s name
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the current number of workers allocated by the consumer
            /// </summary>
            public int WorkersCount { get; set; }

            /// <summary>
            /// Gets or sets all the consumer partition assignments (data received from metrics events)
            /// </summary>
            public IEnumerable<TopicPartitionAssignment> Assignments { get; set; }
        }

        /// <summary>
        /// The response of the telemetry partition consumer response
        /// </summary>
        public class TopicPartitionAssignment
        {
            /// <summary>
            /// Gets or sets the topic name
            /// </summary>
            public string TopicName { get; set; }

            /// <summary>
            /// Gets or sets the instance name
            /// </summary>
            public string InstanceName { get; set; }

            /// <summary>
            /// Gets or sets the current consumer instance status
            /// </summary>
            public string Status { get; set; }

            /// <summary>
            /// Gets or sets the list of running partitions
            /// </summary>
            public IEnumerable<int> RunningPartitions { get; set; }

            /// <summary>
            /// Gets or sets the list of paused partitions
            /// </summary>
            public IEnumerable<int> PausedPartitions { get; set; }

            /// <summary>
            /// Gets or sets the datetime at when the partition assigned was updated
            /// </summary>
            public DateTime LastUpdate { get; set; }
        }
    }
}
