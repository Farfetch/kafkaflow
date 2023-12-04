namespace KafkaFlow.Consumers;

internal class ConsumerMiddlewareContext : IConsumerMiddlewareContext
{
    public IWorker Worker { get; set; }

    public IConsumer Consumer { get; set; }
}
