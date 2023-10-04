namespace KafkaFlow
{
    using System.Threading.Tasks;
    using KafkaFlow.Abstractions;
    using KafkaFlow.Configuration;

    internal class EventHub : IEventHub
    {
        private readonly Event<MessageEventContext> messageConsumeStarted;
        private readonly Event<MessageEventContext> messageProduceStarted;

        public EventHub(ILogHandler log)
        {
            this.messageConsumeStarted = new Event<MessageEventContext>(log);
            this.messageProduceStarted = new Event<MessageEventContext>(log);
        }

        public IEvent<MessageEventContext> MessageConsumeStarted => this.messageConsumeStarted;

        public IEvent<MessageEventContext> MessageProduceStarted => this.messageProduceStarted;

        public Task FireMessageConsumeStartedAsync(MessageEventContext context)
            => this.messageConsumeStarted.FireAsync(context);

        public Task FireMessageProduceStartedAsync(MessageEventContext context)
            => this.messageProduceStarted.FireAsync(context);
    }
}
