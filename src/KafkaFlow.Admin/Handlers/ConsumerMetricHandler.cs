namespace KafkaFlow.Admin.Handlers
{
    using System.Threading.Tasks;
    using KafkaFlow.Admin.Messages;
    using KafkaFlow.TypedHandler;

    internal class ConsumerMetricHandler : IMessageHandler<ConsumerMetric>
    {
        private readonly ITelemetryStorage storage;

        public ConsumerMetricHandler(ITelemetryStorage storage) => this.storage = storage;

        public Task Handle(IMessageContext context, ConsumerMetric message)
        {
            this.storage.Put(message);
            return Task.CompletedTask;
        }
    }
}
