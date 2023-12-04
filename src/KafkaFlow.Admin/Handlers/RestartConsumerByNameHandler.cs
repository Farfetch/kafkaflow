using System.Threading.Tasks;
using KafkaFlow.Admin.Messages;
using KafkaFlow.Consumers;

namespace KafkaFlow.Admin.Handlers;

internal class RestartConsumerByNameHandler : IMessageHandler<RestartConsumerByName>
{
    private readonly IConsumerAccessor _consumerAccessor;

    public RestartConsumerByNameHandler(IConsumerAccessor consumerAccessor) => _consumerAccessor = consumerAccessor;

    public Task Handle(IMessageContext context, RestartConsumerByName message)
    {
        var consumer = _consumerAccessor[message.ConsumerName];

        return consumer?.RestartAsync() ?? Task.CompletedTask;
    }
}
