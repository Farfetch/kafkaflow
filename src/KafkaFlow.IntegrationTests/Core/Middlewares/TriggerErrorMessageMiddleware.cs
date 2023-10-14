namespace KafkaFlow.IntegrationTests.Core.Middlewares
{
    using System.Threading.Tasks;
    using KafkaFlow.IntegrationTests.Core.Exceptions;
    using KafkaFlow.IntegrationTests.Core.Handlers;

    internal class TriggerErrorMessageMiddleware : IMessageMiddleware
    {
        public async Task Invoke(IMessageContext context, MiddlewareDelegate next)
        {
            MessageStorage.Add((byte[])context.Message.Value);
            throw new ErrorExecutingMiddlewareException(nameof(TriggerErrorMessageMiddleware));
            await next(context);
        }
    }
}
