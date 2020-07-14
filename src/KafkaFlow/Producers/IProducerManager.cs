namespace KafkaFlow.Producers
{
    internal interface IProducerManager : IProducerAccessor
    {
        void AddOrUpdate(IMessageProducer producer);
    }
}
