namespace KafkaFlow.Middlewares.ConsumerThrottling
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using KafkaFlow.Middlewares.ConsumerThrottling.Configuration;

    internal class ConsumerThrottlingMiddleware : IMessageMiddleware, IDisposable
    {
        private readonly ConsumerThrottlingConfiguration configuration;
        private readonly IReadOnlyList<IConsumerThrottlingThreshold> thresholds;

        private readonly Timer timer;

        private long metricValue;

        public ConsumerThrottlingMiddleware(ConsumerThrottlingConfiguration configuration)
        {
            this.configuration = configuration;
            this.thresholds = configuration.Thresholds
                .OrderByDescending(x => x.ThresholdValue)
                .ToList();

            this.timer = new Timer(
                state => _ = this.UpdateMetricValueAsync(),
                null,
                configuration.EvaluationInterval,
                configuration.EvaluationInterval);
        }

        public async Task Invoke(IMessageContext context, MiddlewareDelegate next)
        {
            foreach (var threshold in this.thresholds)
            {
                if (await threshold.TryExecuteActionAsync(this.metricValue).ConfigureAwait(false))
                {
                    break;
                }
            }

            await next(context).ConfigureAwait(false);
        }

        public void Dispose()
        {
            this.timer?.Dispose();
        }

        private async Task UpdateMetricValueAsync()
        {
            long total = 0;

            foreach (var metric in this.configuration.Metrics)
            {
                total += await metric.GetValueAsync().ConfigureAwait(false);
            }

            this.metricValue = total;
        }
    }
}
