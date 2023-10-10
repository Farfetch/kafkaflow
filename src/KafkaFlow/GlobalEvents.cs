namespace KafkaFlow
{
    using System.Threading.Tasks;
    using KafkaFlow.Configuration;

    internal class GlobalEvents : IGlobalEvents
    {
        private readonly Event<MessageEventContext> messageConsumeStarted;

        private readonly Event<MessageEventContext> messageConsumeCompleted;

        private readonly Event<MessageEventContext> messageProduceStarted;

        private readonly Event<MessageEventContext> messageProduceCompleted;

        public GlobalEvents(ILogHandler log)
        {
            this.messageConsumeStarted = new(log);
            this.messageConsumeCompleted = new(log);
            this.messageProduceStarted = new(log);
            this.messageProduceCompleted = new(log);
        }

        public IEvent<MessageEventContext> MessageConsumeStarted => this.messageConsumeStarted;

        public IEvent<MessageEventContext> MessageConsumeCompleted => this.messageConsumeCompleted;

        public IEvent<MessageEventContext> MessageProduceStarted => this.messageProduceStarted;

        public IEvent<MessageEventContext> MessageProduceCompleted => this.messageProduceCompleted;

        public Task FireMessageConsumeStartedAsync(MessageEventContext context)
            => this.messageConsumeStarted.FireAsync(context);

        public Task FireMessageConsumeCompletedAsync(MessageEventContext context)
            => this.messageConsumeCompleted.FireAsync(context);

        public Task FireMessageProduceStartedAsync(MessageEventContext context)
            => this.messageProduceStarted.FireAsync(context);

        public Task FireMessageProduceCompletedAsync(MessageEventContext context)
            => this.messageProduceCompleted.FireAsync(context);
    }
}
