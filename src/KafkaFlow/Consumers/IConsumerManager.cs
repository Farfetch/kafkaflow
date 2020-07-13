namespace KafkaFlow.Consumers
{
    internal interface IConsumerManager : IConsumerAccessor
    {
        void AddOrUpdate(IMessageConsumer consumer);
    }
}
