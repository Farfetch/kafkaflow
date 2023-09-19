namespace KafkaFlow.Admin.Handlers
{
    using System.Threading.Tasks;
    using KafkaFlow.Admin.Messages;
    using KafkaFlow.Middlewares.TypedHandler;

    internal class ConsumerTelemetryMetricHandler : IMessageHandler<ConsumerTelemetryMetric>
    {
        private readonly ITelemetryStorage storage;

        public ConsumerTelemetryMetricHandler(ITelemetryStorage storage) => this.storage = storage;

        public Task Handle(IMessageContext context, ConsumerTelemetryMetric message)
        {
            this.storage.Put(message);
            return Task.CompletedTask;
        }
    }
}
