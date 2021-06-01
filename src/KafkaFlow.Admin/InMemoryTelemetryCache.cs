namespace KafkaFlow.Admin
{
    using System.Collections.Generic;
    using KafkaFlow.Admin.Messages;
    using Microsoft.Extensions.Caching.Memory;

    /// <summary>
    /// Provide cache operations related to telemetry data
    /// </summary>
    public class InMemoryTelemetryCache : ITelemetryCache
    {
        private readonly IMemoryCache cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryTelemetryCache"/> class.
        /// </summary>
        /// <param name="cache">The memory cache class to manage telemetry data</param>
        public InMemoryTelemetryCache(IMemoryCache cache) => this.cache = cache;

        /// <inheritdoc />
        public List<ConsumerMetric> Get(string groupId, string consumerName)
        {
            return this.cache.TryGetValue(this.BuildKey(groupId, consumerName), out List<ConsumerMetric> metric) ?
                metric :
                new List<ConsumerMetric>();
        }

        /// <inheritdoc />
        public void Put(string groupId, string consumerName, ConsumerMetric metric)
        {
            var entry = this.Get(groupId, consumerName);

            entry.RemoveAll(e => e.HostName == metric.HostName);
            entry.Add(metric);
            this.cache.Set(this.BuildKey(groupId, consumerName), entry);
        }

        private string BuildKey(string groupId, string consumerName) => $"{groupId}-{consumerName}";
    }
}
