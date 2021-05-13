namespace KafkaFlow.Admin.WebApi.Contracts
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The consumer partition assignment
    /// </summary>
    public class PartitionAssignment
    {
        /// <summary>
        /// Gets or sets the topic name
        /// </summary>
        public string Topic { get; set; }

        /// <summary>
        /// Gets or sets the host name
        /// </summary>
        public string HostName { get; set; }

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
