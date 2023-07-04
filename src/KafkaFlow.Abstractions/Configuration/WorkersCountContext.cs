namespace KafkaFlow.Configuration
{
    using System.Collections.Generic;

    /// <summary>
    /// A metadata class with some context information help to calculate the number of workers
    /// </summary>
    public class WorkersCountContext
    {
        public WorkersCountContext(
            IDependencyResolver dependencyResolver,
            string consumerName,
            string consumerGroupId,
            IReadOnlyCollection<TopicPartitions> assignedPartitions)
        {
            this.DependencyResolver = dependencyResolver;
            this.ConsumerName = consumerName;
            this.ConsumerGroupId = consumerGroupId;
            this.AssignedPartitions = assignedPartitions;
        }

        /// <summary>
        /// Gets the dependency injection container abstraction
        /// </summary>
        public IDependencyResolver DependencyResolver { get; }

        public string ConsumerName { get; }

        public string ConsumerGroupId { get; }

        /// <summary>
        /// Gets the assigned partitions to the consumer
        /// </summary>
        public IReadOnlyCollection<TopicPartitions> AssignedPartitions { get; }
    }
}
