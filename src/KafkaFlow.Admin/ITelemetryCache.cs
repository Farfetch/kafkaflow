namespace KafkaFlow.Admin
{
    using System.Collections.Generic;
    using KafkaFlow.Admin.Messages;

    /// <summary>
    /// Used to implement a cache provider to manage telemetry data
    /// </summary>
    public interface ITelemetryCache
    {
        /// <summary>
        /// Gets the cached metric indexed with the parameters provided
        /// </summary>
        /// <param name="groupId">The group id</param>
        /// <param name="consumerName">The consumer name</param>
        /// <returns>The list of consumer metrics stored in the cache</returns>
        List<ConsumerMetric> Get(string groupId, string consumerName);

        /// <summary>
        /// Puts in the cache the metric provided
        /// </summary>
        /// <param name="groupId">The group id</param>
        /// <param name="consumerName">The consumer name</param>
        /// <param name="metric">The consumer metric</param>
        void Put(string groupId, string consumerName, ConsumerMetric metric);
    }
}
