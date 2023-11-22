namespace KafkaFlow
{
    using System.Threading.Tasks;
    using KafkaFlow.Configuration;

    internal class GlobalEvents : IGlobalEvents
    {
        private readonly Event<MessageEventContext> messageConsumeCompleted;
        private readonly Event<MessageErrorEventContext> messageConsumeError;
        private readonly Event<MessageEventContext> messageConsumeStarted;
        private readonly Event<MessageEventContext> messageProduceCompleted;
        private readonly Event<MessageErrorEventContext> messageProduceError;
        private readonly Event<MessageEventContext> messageProduceStarted;

        public GlobalEvents(ILogHandler log)
        {
            this.messageConsumeCompleted = new(log);
            this.messageConsumeError = new(log);
            this.messageConsumeStarted = new(log);
            this.messageProduceCompleted = new(log);
            this.messageProduceError = new(log);
            this.messageProduceStarted = new(log);
        }

        public IEvent<MessageEventContext> MessageConsumeCompleted => this.messageConsumeCompleted;

        public IEvent<MessageErrorEventContext> MessageConsumeError => this.messageConsumeError;

        public IEvent<MessageEventContext> MessageConsumeStarted => this.messageConsumeStarted;

        public IEvent<MessageEventContext> MessageProduceCompleted => this.messageProduceCompleted;

        public IEvent<MessageErrorEventContext> MessageProduceError => this.messageProduceError;

        public IEvent<MessageEventContext> MessageProduceStarted => this.messageProduceStarted;

        public Task FireMessageConsumeStartedAsync(MessageEventContext context)
            => this.messageConsumeStarted.FireAsync(context);

        public Task FireMessageConsumeErrorAsync(MessageErrorEventContext context)
            => this.messageConsumeError.FireAsync(context);

        public Task FireMessageConsumeCompletedAsync(MessageEventContext context)
            => this.messageConsumeCompleted.FireAsync(context);

        public Task FireMessageProduceStartedAsync(MessageEventContext context)
            => this.messageProduceStarted.FireAsync(context);

        public Task FireMessageProduceErrorAsync(MessageErrorEventContext context)
           => this.messageProduceError.FireAsync(context);

        public Task FireMessageProduceCompletedAsync(MessageEventContext context)
            => this.messageProduceCompleted.FireAsync(context);

    }
}
