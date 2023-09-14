namespace KafkaFlow.Configuration
{
    using System;
    using System.Collections.Generic;
    using Confluent.Kafka;

    /// <summary>
    /// Represents a handler for pending offsets statistics.
    /// </summary>
    public class PendingOffsetsStatisticsHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PendingOffsetsStatisticsHandler"/> class with the specified handler and interval.
        /// </summary>
        /// <param name="handler">The action to handle pending offsets statistics.</param>
        /// <param name="interval">The interval at which the handler should be executed.</param>
        public PendingOffsetsStatisticsHandler(Action<IDependencyResolver, IEnumerable<TopicPartitionOffset>> handler, TimeSpan interval)
        {
            this.Handler = handler;
            this.Interval = interval;
        }

        /// <summary>
        /// Gets the action that handles pending offsets statistics.
        /// </summary>
        public Action<IDependencyResolver, IEnumerable<TopicPartitionOffset>> Handler { get; }

        /// <summary>
        /// Gets the interval at which the handler should be executed.
        /// </summary>
        public TimeSpan Interval { get; }
    }
}
