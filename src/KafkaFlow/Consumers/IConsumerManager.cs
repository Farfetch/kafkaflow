namespace KafkaFlow.Consumers
{
    internal interface IConsumerManager
    {
        void AddOrUpdateConsumer(IMessageConsumer consumer);
    }
}
