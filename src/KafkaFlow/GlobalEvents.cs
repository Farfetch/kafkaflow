using System.Threading.Tasks;
using KafkaFlow.Configuration;

namespace KafkaFlow
{
    internal class GlobalEvents : IGlobalEvents
    {
        private readonly Event<MessageEventContext> _messageConsumeCompleted;
        private readonly Event<MessageErrorEventContext> _messageConsumeError;
        private readonly Event<MessageEventContext> _messageConsumeStarted;
        private readonly Event<MessageEventContext> _messageProduceCompleted;
        private readonly Event<MessageErrorEventContext> _messageProduceError;
        private readonly Event<MessageEventContext> _messageProduceStarted;

        public GlobalEvents(ILogHandler log)
        {
            _messageConsumeCompleted = new(log);
            _messageConsumeError = new(log);
            _messageConsumeStarted = new(log);
            _messageProduceCompleted = new(log);
            _messageProduceError = new(log);
            _messageProduceStarted = new(log);
        }

        public IEvent<MessageEventContext> MessageConsumeCompleted => _messageConsumeCompleted;

        public IEvent<MessageErrorEventContext> MessageConsumeError => _messageConsumeError;

        public IEvent<MessageEventContext> MessageConsumeStarted => _messageConsumeStarted;

        public IEvent<MessageEventContext> MessageProduceCompleted => _messageProduceCompleted;

        public IEvent<MessageErrorEventContext> MessageProduceError => _messageProduceError;

        public IEvent<MessageEventContext> MessageProduceStarted => _messageProduceStarted;

        public Task FireMessageConsumeStartedAsync(MessageEventContext context)
            => _messageConsumeStarted.FireAsync(context);

        public Task FireMessageConsumeErrorAsync(MessageErrorEventContext context)
            => _messageConsumeError.FireAsync(context);

        public Task FireMessageConsumeCompletedAsync(MessageEventContext context)
            => _messageConsumeCompleted.FireAsync(context);

        public Task FireMessageProduceStartedAsync(MessageEventContext context)
            => _messageProduceStarted.FireAsync(context);

        public Task FireMessageProduceErrorAsync(MessageErrorEventContext context)
           => _messageProduceError.FireAsync(context);

        public Task FireMessageProduceCompletedAsync(MessageEventContext context)
            => _messageProduceCompleted.FireAsync(context);
    }
}
