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
        /// Gets all the consumer telemetry metrics
        /// </summary>
        /// <returns>The list of consumer metrics</returns>
        IEnumerable<ConsumerTelemetryMetric> Get();

        /// <summary>
        /// Store the metric provided
        /// </summary>
        /// <param name="telemetryMetric">The consumer telemetry metric</param>
        void Put(ConsumerTelemetryMetric telemetryMetric);
    }
}
