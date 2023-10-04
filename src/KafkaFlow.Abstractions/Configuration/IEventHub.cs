namespace KafkaFlow.Configuration
{
    using KafkaFlow.Abstractions;

    public interface IEventHub
    {
        IEvent<MessageEventContext> MessageConsumeStarted { get; }

        IEvent<MessageEventContext> MessageProduceStarted { get; }
    }
}
