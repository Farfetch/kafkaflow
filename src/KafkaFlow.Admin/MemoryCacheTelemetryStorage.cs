namespace KafkaFlow.Admin
{
    using System.Collections.Generic;
    using KafkaFlow.Admin.Messages;
    using Microsoft.Extensions.Caching.Memory;

    internal class MemoryCacheTelemetryStorage : ITelemetryStorage
    {
        private readonly IMemoryCache cache;

        public MemoryCacheTelemetryStorage(IMemoryCache cache) => this.cache = cache;

        public List<ConsumerMetric> Get(string groupId, string consumerName)
        {
            return this.cache.TryGetValue(this.BuildKey(groupId, consumerName), out List<ConsumerMetric> metric) ?
                metric :
                new List<ConsumerMetric>();
        }

        public void Put(string groupId, string consumerName, ConsumerMetric metric)
        {
            var entry = this.Get(groupId, consumerName);

            entry.RemoveAll(e => e.InstanceName == metric.InstanceName);
            entry.Add(metric);
            this.cache.Set(this.BuildKey(groupId, consumerName), entry);
        }

        private string BuildKey(string groupId, string consumerName) => $"{groupId}-{consumerName}";
    }
}
