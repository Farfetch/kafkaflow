using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using KafkaFlow.Admin.Messages;

namespace KafkaFlow.Admin;

internal class MemoryTelemetryStorage : ITelemetryStorage
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly TimeSpan _cleanRunInterval;
    private readonly TimeSpan _expiryTime;
    private readonly object _cleanSyncRoot = new();

    private readonly ConcurrentDictionary<(string, string, string, string), ConsumerTelemetryMetric> _metrics = new();

    private DateTime _lastCleanDate;

    public MemoryTelemetryStorage(TimeSpan cleanRunInterval, TimeSpan expiryTime, IDateTimeProvider dateTimeProvider)
    {
        _cleanRunInterval = cleanRunInterval;
        _expiryTime = expiryTime;
        _dateTimeProvider = dateTimeProvider;
        _lastCleanDate = dateTimeProvider.MinValue;
    }

    public IEnumerable<ConsumerTelemetryMetric> Get() => _metrics.Values;

    public void Put(ConsumerTelemetryMetric telemetryMetric)
    {
        this.TryCleanItems();
        _metrics[BuildKey(telemetryMetric)] = telemetryMetric;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static (string, string, string, string) BuildKey(ConsumerTelemetryMetric telemetryMetric) =>
        (telemetryMetric.InstanceName, telemetryMetric.GroupId, telemetryMetric.ConsumerName, telemetryMetric.Topic);

    private void TryCleanItems()
    {
        if (!this.NeedsCleaning())
        {
            return;
        }

        lock (_cleanSyncRoot)
        {
            if (!this.NeedsCleaning())
            {
                return;
            }

            _lastCleanDate = _dateTimeProvider.UtcNow;

            this.CleanExpiredItems();
        }
    }

    private void CleanExpiredItems()
    {
        foreach (var (key, metric) in _metrics.Select(x => (x.Key, x.Value)))
        {
            if (_dateTimeProvider.UtcNow - metric.SentAt > _expiryTime)
            {
                _metrics.TryRemove(key, out _);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool NeedsCleaning() => _dateTimeProvider.UtcNow - _lastCleanDate > _cleanRunInterval;
}
