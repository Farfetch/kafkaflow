using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KafkaFlow.Middlewares.ConsumerThrottling.Configuration;

namespace KafkaFlow.Middlewares.ConsumerThrottling;

internal class ConsumerThrottlingMiddleware : IMessageMiddleware, IDisposable
{
    private readonly ConsumerThrottlingConfiguration _configuration;
    private readonly IReadOnlyList<IConsumerThrottlingThreshold> _thresholds;

    private readonly Timer _timer;

    private long _metricValue;

    public ConsumerThrottlingMiddleware(ConsumerThrottlingConfiguration configuration)
    {
        _configuration = configuration;
        _thresholds = configuration.Thresholds
            .OrderByDescending(x => x.ThresholdValue)
            .ToList();

        _timer = new Timer(
            state => _ = this.UpdateMetricValueAsync(),
            null,
            configuration.EvaluationInterval,
            configuration.EvaluationInterval);
    }

    public async Task Invoke(IMessageContext context, MiddlewareDelegate next)
    {
        foreach (var threshold in _thresholds)
        {
            if (await threshold.TryExecuteActionAsync(_metricValue).ConfigureAwait(false))
            {
                break;
            }
        }

        await next(context).ConfigureAwait(false);
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }

    private async Task UpdateMetricValueAsync()
    {
        long total = 0;

        foreach (var metric in _configuration.Metrics)
        {
            total += await metric.GetValueAsync().ConfigureAwait(false);
        }

        _metricValue = total;
    }
}
