namespace KafkaFlow.Admin
{
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using KafkaFlow.Admin.Messages;

    internal class MemoryTelemetryStorage : ITelemetryStorage
    {
        private readonly Dictionary<(string, string, string), ConsumerMetric> metrics = new();

        public IReadOnlyCollection<ConsumerMetric> Get() => this.metrics.Values;

        public void Put(ConsumerMetric metric)
        {
            this.metrics[BuildKey(metric)] = metric;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static (string, string, string) BuildKey(ConsumerMetric metric) =>
            (metric.InstanceName, metric.GroupId, metric.ConsumerName);
    }
}
