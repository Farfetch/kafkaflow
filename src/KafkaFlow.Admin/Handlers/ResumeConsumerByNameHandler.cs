using System.Linq;
using System.Threading.Tasks;
using KafkaFlow.Admin.Extensions;
using KafkaFlow.Admin.Messages;
using KafkaFlow.Consumers;

namespace KafkaFlow.Admin.Handlers;

internal class ResumeConsumerByNameHandler : IMessageHandler<ResumeConsumerByName>
{
    private readonly IConsumerAccessor _consumerAccessor;

    public ResumeConsumerByNameHandler(IConsumerAccessor consumerAccessor) => _consumerAccessor = consumerAccessor;

    public Task Handle(IMessageContext context, ResumeConsumerByName message)
    {
        var consumer = _consumerAccessor[message.ConsumerName];

        var assignment = consumer.FilterAssigment(message.Topics);

        if (assignment.Any())
        {
            consumer?.Resume(assignment);
        }

        return Task.CompletedTask;
    }
}
