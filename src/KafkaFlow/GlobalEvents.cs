namespace KafkaFlow
{
    using System.Threading.Tasks;
    using KafkaFlow.Configuration;

    internal class GlobalEvents : IGlobalEvents
    {
        private readonly Event<MessageEventContext> messageConsumeStarted;

        private readonly Event<MessageEventContext> messageConsumeStopped;

        private readonly Event<MessageEventContext> messageProduceStarted;

        public GlobalEvents(ILogHandler log)
        {
            this.messageConsumeStarted = new(log);
            this.messageConsumeStopped = new(log);
            this.messageProduceStarted = new(log);
        }

        public IEvent<MessageEventContext> MessageConsumeStarted => this.messageConsumeStarted;

        public IEvent<MessageEventContext> MessageConsumeEnded => this.messageConsumeStopped;

        public IEvent<MessageEventContext> MessageProduceStarted => this.messageProduceStarted;

        public Task FireMessageConsumeStartedAsync(MessageEventContext context)
            => this.messageConsumeStarted.FireAsync(context);

        public Task FireMessageConsumeCompletedAsync(MessageEventContext context)
            => this.messageConsumeStopped.FireAsync(context);

        public Task FireMessageProduceStartedAsync(MessageEventContext context)
            => this.messageProduceStarted.FireAsync(context);
    }
}
