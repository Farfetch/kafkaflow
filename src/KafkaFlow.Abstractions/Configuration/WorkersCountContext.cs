namespace KafkaFlow.Configuration
{
    using System.Collections.Generic;

    /// <summary>
    /// A metadata class with some context information help to calculate the number of workers
    /// </summary>
    public class WorkersCountContext
    {
        public WorkersCountContext(IReadOnlyCollection<TopicPartitions> partitions)
        {
            this.Partitions = partitions;
        }

        /// <summary>
        /// The partitions assigned to the consumer
        /// </summary>
        public IReadOnlyCollection<TopicPartitions> Partitions { get; }
    }
}
