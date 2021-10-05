namespace KafkaFlow.Extensions.ProducerThrottling.Actions
{
    using System.Linq;
    using System.Threading.Tasks;
    using KafkaFlow.Consumers;

    /// <summary>
    /// Pauses the consumer when the threshold was reached or resumes when it wasn't.
    /// </summary>'
    public class PauseConsumerAction : IAction
    {
        private readonly string consumerName;
        private readonly IMessageConsumer consumer;
        private readonly ILogHandler log;

        /// <summary>
        /// Initializes a new instance of the <see cref="PauseConsumerAction"/> class
        /// </summary>
        /// <param name="resolver">Instance of <see cref="IDependencyResolver"/></param>
        /// <param name="consumerName">The consumer name</param>
        public PauseConsumerAction(IDependencyResolver resolver, string consumerName)
        {
            this.consumerName = consumerName;
            this.consumer = resolver.Resolve<IConsumerAccessor>().GetConsumer(consumerName);
            this.log = resolver.Resolve<ILogHandler>();
        }

        /// <inheritdoc />
        public Task OnThresholdReachedHandler(
            IMessageContext context,
            long currentValue,
            long threshold)
        {
            if (this.consumer.RunningPartitions.Any())
            {
                this.consumer.Pause();
                this.log.Info(
                    $"The consumer was paused due to threshold value has been reached'",
                    new
                    {
                        this.consumerName,
                        threshold,
                        currentValue,
                        topicToPublish = context.ProducerContext.Topic,
                    });
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task OnThresholdNotReachedHandler(
            IMessageContext context,
            long currentValue,
            long threshold)
        {
            if (this.consumer.PausedPartitions.Any())
            {
                this.consumer.Resume();
                this.log.Info(
                    $"The consumer was resumed due to threshold value has not been reached'",
                    new
                    {
                        this.consumerName,
                        threshold,
                        currentValue,
                        topicToPublish = context.ProducerContext.Topic,
                    });
            }

            return Task.CompletedTask;
        }
    }
}
