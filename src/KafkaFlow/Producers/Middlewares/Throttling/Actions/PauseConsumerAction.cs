namespace KafkaFlow.Producers.Middlewares.Throttling.Actions
{
    using System.Linq;
    using System.Threading.Tasks;
    using KafkaFlow.Consumers;

    internal class PauseConsumerAction : IAction
    {
        private readonly IMessageConsumer consumer;

        public PauseConsumerAction(IConsumerAccessor consumerAccessor, int threshold, string consumerName)
        {
            this.Threshold = threshold;
            this.consumer = consumerAccessor.GetConsumer(consumerName);
        }

        public int Threshold { get; }

        public Task HandleAsync(IMessageContext context)
        {
            if (!this.consumer.RunningPartitions.Any())
            {
                return Task.CompletedTask;
            }

            this.consumer.Pause();

            return Task.CompletedTask;
        }

        public Task OnEndAsync(IMessageContext context)
        {
            if (!this.consumer.PausedPartitions.Any())
            {
                return Task.CompletedTask;
            }

            this.consumer.Resume();

            return Task.CompletedTask;
        }
    }
}
