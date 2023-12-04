using System.Threading.Tasks;
using KafkaFlow.Admin.Messages;

namespace KafkaFlow.Admin.Handlers;

internal class ConsumerTelemetryMetricHandler : IMessageHandler<ConsumerTelemetryMetric>
{
    private readonly ITelemetryStorage _storage;

    public ConsumerTelemetryMetricHandler(ITelemetryStorage storage) => _storage = storage;

    public Task Handle(IMessageContext context, ConsumerTelemetryMetric message)
    {
        _storage.Put(message);
        return Task.CompletedTask;
    }
}
