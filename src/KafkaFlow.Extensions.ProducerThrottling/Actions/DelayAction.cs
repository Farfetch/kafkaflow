namespace KafkaFlow.Extensions.ProducerThrottling.Actions
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Executes a Task.Delay when the threshold was reached.
    /// </summary>
    public class DelayAction : IAction
    {
        private readonly TimeSpan delay;
        private readonly ILogHandler log;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelayAction"/> class
        /// </summary>
        /// <param name="resolver">Instance of <see cref="IDependencyResolver"/></param>
        /// <param name="delay">The interval of time to wait.</param>
        public DelayAction(IDependencyResolver resolver, TimeSpan delay)
        {
            this.delay = delay;
            this.log = resolver.Resolve<ILogHandler>();
        }

        /// <inheritdoc />
        public Task OnThresholdReachedHandler(
            IMessageContext context,
            long currentValue,
            long threshold)
        {
            this.log.Info(
                $"The publishing will be postponed due to threshold value has been reached'",
                new
                {
                    this.delay.TotalMilliseconds,
                    threshold,
                    currentValue,
                    topicToPublish = context.ProducerContext.Topic,
                });

            return Task.Delay(this.delay, context.ProducerContext.ClientStopped);
        }

        /// <inheritdoc />
        public Task OnThresholdNotReachedHandler(
            IMessageContext context,
            long currentValue,
            long threshold)
        {
            return Task.CompletedTask;
        }
    }
}
