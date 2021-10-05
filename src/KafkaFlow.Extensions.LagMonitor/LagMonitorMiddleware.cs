namespace KafkaFlow.Extensions.ProducerThrottling
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Client.Metrics;
    using KafkaFlow.Client.Producers;

    internal abstract class LagMonitorMiddleware : IMessageMiddleware
    {
        protected readonly long LagThreshold;
        protected readonly ILogHandler Log;

        private readonly string topicToObserve;
        private readonly string consumerGroupToObserve;
        private readonly ILagReader lagReader;
        private readonly TimeSpan frequency;
        private readonly Stopwatch timer;
        private Func<IMessageContext, long, Task> onThresholdReachedHandler = (_, _) => Task.CompletedTask;
        private Func<IMessageContext, long, Task> onThresholdNotReachedHandler = (_, _) => Task.CompletedTask;

        public LagMonitorMiddleware(
            string topicToObserve,
            string consumerGroupToObserve,
            long lagThreshold,
            TimeSpan frequency,
            ILagReader lagReader,
            ILogHandler log)
        {
            this.topicToObserve = topicToObserve;
            this.consumerGroupToObserve = consumerGroupToObserve;
            this.LagThreshold = lagThreshold;
            this.frequency = frequency;
            this.Log = log;
            this.lagReader = lagReader;
            this.timer = Stopwatch.StartNew();
        }

        protected void OnThresholdReachedHandler(Func<IMessageContext, long, Task> handler)
        {
            this.onThresholdReachedHandler = handler;
        }

        protected void OnThresholdNotReachedHandler(Func<IMessageContext, long, Task> handler)
        {
            this.onThresholdNotReachedHandler = handler;
        }

        public async Task Invoke(IMessageContext context, MiddlewareDelegate next)
        {
            if (this.timer.Elapsed > this.frequency)
            {
                while (true)
                {
                    var currentLag = await this.lagReader.GetLagAsync(this.topicToObserve, this.consumerGroupToObserve);

                    this.timer.Restart();

                    if (currentLag < this.LagThreshold)
                    {
                        await this.onThresholdNotReachedHandler(context, currentLag).ConfigureAwait(false);
                        break;
                    }

                    this.Log.Info(
                        $"Lag value on topic '{this.topicToObserve}' and consumer-group '{this.consumerGroupToObserve}' is above the threshold configured",
                        new
                        {
                            observedTopic = this.topicToObserve,
                            observedConsumerGroup = this.consumerGroupToObserve,
                            lag = currentLag,
                            threshold = this.LagThreshold,
                            topicToPublish = context.ProducerContext.Topic,
                            MessageKey = context.Message.Key,
                            MessageValue = context.Message.Value,
                        });

                    await this.onThresholdReachedHandler(context, currentLag).ConfigureAwait(false);

                    await Task.Delay(this.frequency);
                }
            }

            await next(context);
        }
    }
}
