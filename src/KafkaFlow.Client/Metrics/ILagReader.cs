namespace KafkaFlow.Client.Metrics
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Reader of lag metrics
    /// </summary>
    public interface ILagReader
    {
        /// <summary>
        /// Gets the lag of a consumerGroup in a specific topic
        /// </summary>
        /// <param name="topic">The topic name</param>
        /// <param name="consumerGroup">The consumer group</param>
        /// <returns></returns>
        Task<IEnumerable<PartitionLag>> GetLagAsync(
            string topic,
            string consumerGroup);
    }
}
