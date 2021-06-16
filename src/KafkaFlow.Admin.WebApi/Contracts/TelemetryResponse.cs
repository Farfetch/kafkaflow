namespace KafkaFlow.Admin.WebApi.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Newtonsoft.Json;

    /// <summary>
    /// The response of telemetry data
    /// </summary>
    public class TelemetryResponse
    {
        /// <summary>
        /// Gets or sets the consumer group collection
        /// </summary>
        public IEnumerable<ConsumerGroup> Groups { get; set; }

        /// <summary>
        /// The response of the consumer group
        /// </summary>
        public class ConsumerGroup
        {
            /// <summary>
            /// Gets or sets the consumer group id
            /// </summary>
            [Required]
            [JsonProperty(Required = Required.DisallowNull)]
            public string GroupId { get; set; }

            /// <summary>
            /// Gets or sets the consumers collection
            /// </summary>
            public IEnumerable<Consumer> Consumers { get; set; }
        }

        /// <summary>
        /// The response of the consumer
        /// </summary>
        public class Consumer
        {
            /// <summary>
            /// Gets or sets the consumer´s name
            /// </summary>
            [Required]
            [JsonProperty(Required = Required.DisallowNull)]
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the current number of workers allocated by the consumer
            /// </summary>
            public int WorkersCount { get; set; }

            /// <summary>
            /// Gets or sets all the consumer partition assignments
            /// </summary>
            public IEnumerable<TopicPartitionAssignment> Assignments { get; set; }
        }

        /// <summary>
        /// The response of the topic partition consumer assignment
        /// </summary>
        public class TopicPartitionAssignment
        {
            /// <summary>
            /// Gets or sets the topic name
            /// </summary>
            [Required]
            [JsonProperty(Required = Required.DisallowNull)]
            public string TopicName { get; set; }

            /// <summary>
            /// Gets or sets the instance name
            /// </summary>
            [Required]
            [JsonProperty(Required = Required.DisallowNull)]
            public string InstanceName { get; set; }

            /// <summary>
            /// Gets or sets the current consumer instance status
            /// </summary>
            [Required]
            [JsonProperty(Required = Required.DisallowNull)]
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
