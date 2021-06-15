namespace KafkaFlow.Admin
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using KafkaFlow.Admin.Messages;

    internal class MemoryTelemetryStorage : ITelemetryStorage
    {
        private readonly IDateTimeProvider dateTimeProvider;
        private readonly TimeSpan cleanRunInterval;
        private readonly TimeSpan expiryTime;
        private readonly object cleanSyncRoot = new();

        private readonly ConcurrentDictionary<(string, string, string), ConsumerMetric> metrics = new();

        private DateTime lastCleanDate;

        public MemoryTelemetryStorage(TimeSpan cleanRunInterval, TimeSpan expiryTime, IDateTimeProvider dateTimeProvider)
        {
            this.cleanRunInterval = cleanRunInterval;
            this.expiryTime = expiryTime;
            this.dateTimeProvider = dateTimeProvider;
            this.lastCleanDate = dateTimeProvider.MinValue;
        }

        public IEnumerable<ConsumerMetric> Get() => this.metrics.Values;

        public void Put(ConsumerMetric metric)
        {
            this.TryCleanItems();
            this.metrics[BuildKey(metric)] = metric;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static (string, string, string) BuildKey(ConsumerMetric metric) =>
            (metric.InstanceName, metric.GroupId, metric.ConsumerName);

        private void TryCleanItems()
        {
            if (!this.NeedsClean())
            {
                return;
            }

            lock (this.cleanSyncRoot)
            {
                if (!this.NeedsClean())
                {
                    return;
                }

                this.lastCleanDate = this.dateTimeProvider.Now;

                this.CleanExpiredItems();
            }
        }

        private void CleanExpiredItems()
        {
            foreach (var metric in this.metrics.ToList())
            {
                if (this.dateTimeProvider.Now - metric.Value.SentAt > this.expiryTime)
                {
                    this.metrics.TryRemove(metric.Key, out _);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool NeedsClean() => this.dateTimeProvider.Now - this.lastCleanDate > this.cleanRunInterval;
    }
}
