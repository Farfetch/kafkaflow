namespace KafkaFlow.Extensions.ProducerThrottling
{
    using System;
    using System.Threading.Tasks;
    using Client.Metrics;

    internal class DelayMiddleware : LagMonitorMiddleware
    {
        public DelayMiddleware(
            string topicToObserve,
            string consumerGroupToObserve,
            long lagThreshold,
            TimeSpan frequency,
            TimeSpan delay,
            ILagReader lagReader,
            ILogHandler log)
            : base(
                topicToObserve,
                consumerGroupToObserve,
                lagThreshold,
                frequency,
                lagReader,
                log)
        {
            this.OnThresholdReachedHandler((context, _) =>
            {
                this.Log.Info(
                    $"The publishing was postponed due to max lag threshold been reached'",
                    new
                    {
                        delayInSec = delay.TotalSeconds,
                        threshold = this.LagThreshold,
                        topicToPublish = context.ProducerContext.Topic,
                        MessageKey = context.Message.Key,
                        MessageValue = context.Message.Value,
                    });

                return Task.Delay(delay, context.ProducerContext.ClientStopped);
            });
        }
    }
}
