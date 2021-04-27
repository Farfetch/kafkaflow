namespace KafkaFlow.IntegrationTests.Core.Middlewares
{
    using System.Threading.Tasks;
    using KafkaFlow.IntegrationTests.Core.Handlers;

    internal class GzipMiddleware : IMessageMiddleware
    {
        public async Task Invoke(IMessageContext context, MiddlewareDelegate next)
        {
            MessageStorage.Add((byte[]) context.Message.Value);
            await next(context);
        }
    }
}
