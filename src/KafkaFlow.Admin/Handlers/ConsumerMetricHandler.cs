namespace KafkaFlow.Admin.Handlers
{
    using System.Threading.Tasks;
    using KafkaFlow.Admin.Messages;
    using KafkaFlow.TypedHandler;

    internal class ConsumerMetricHandler : IMessageHandler<ConsumerMetric>
    {
        private readonly ITelemetryCache cache;

        public ConsumerMetricHandler(ITelemetryCache cache) => this.cache = cache;

        public Task Handle(IMessageContext context, ConsumerMetric message)
        {
            this.cache.Put(message.GroupId, message.ConsumerName, message);
            return Task.CompletedTask;
        }
    }
}
