namespace KafkaFlow.Admin
{
    using System.Collections.Generic;
    using KafkaFlow.Admin.Messages;

    /// <summary>
    /// Used to implement a telemetry data storage provider
    /// </summary>
    public interface ITelemetryStorage
    {
        /// <summary>
        /// Gets the stored metric indexed with the parameters provided
        /// </summary>
        /// <returns>The list of consumer metrics stored in the cache</returns>
        IEnumerable<ConsumerMetric> Get();

        /// <summary>
        /// Store the metric provided
        /// </summary>
        /// <param name="metric">The consumer metric</param>
        void Put(ConsumerMetric metric);
    }
}
