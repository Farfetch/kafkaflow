namespace KafkaFlow.Extensions.ProducerThrottling
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Client.Metrics;
    using Consumers;

    internal class PauseConsumerMiddleware : LagMonitorMiddleware
    {
        public PauseConsumerMiddleware(
            string topicToObserve,
            string consumerGroupToObserve,
            long lagThreshold,
            TimeSpan frequency,
            IMessageConsumer consumer,
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
            this.OnThresholdReachedHandler((_, _) =>
            {
                if (consumer.RunningPartitions.Any())
                {
                    consumer.Pause();
                }

                return Task.CompletedTask;
            });

            this.OnThresholdNotReachedHandler((_, _) =>
            {
                if (consumer.PausedPartitions.Any())
                {
                    consumer.Resume();
                }

                return Task.CompletedTask;
            });
        }
    }
}

