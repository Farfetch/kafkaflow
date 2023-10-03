namespace KafkaFlow
{
    using System.Threading.Tasks;
    using KafkaFlow.Configuration;

    internal class EventHub : IEventHub
    {
        private readonly Event<IMessageContext> messageConsumeStarted;

        public EventHub(ILogHandler log)
        {
            this.messageConsumeStarted = new Event<IMessageContext>(log);
        }

        public IEvent<IMessageContext> MessageConsumeStarted => this.messageConsumeStarted;

        public Task FireMessageConsumeStartedAsync(IMessageContext context, IDependencyResolver resolver)
            => this.messageConsumeStarted.FireAsync(context);
    }
}
