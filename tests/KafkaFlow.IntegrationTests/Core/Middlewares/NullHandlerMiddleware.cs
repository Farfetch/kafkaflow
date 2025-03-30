using System.Threading.Tasks;
using KafkaFlow.IntegrationTests.Core.Handlers;

namespace KafkaFlow.IntegrationTests.Core.Middlewares;

public sealed class NullHandlerMiddleware : IMessageMiddleware
{
    public Task Invoke(IMessageContext context, MiddlewareDelegate next)
    {
        if (context.Message.Value is null)
        {
            MessageStorage.AddNullMessage(context.Message);
        }
        return next(context);
    }
}
