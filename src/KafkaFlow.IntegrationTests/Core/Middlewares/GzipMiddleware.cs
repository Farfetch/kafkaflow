namespace KafkaFlow.IntegrationTests.Core.Middlewares
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using KafkaFlow.IntegrationTests.Core.Handlers;

    internal class GzipMiddleware : IMessageMiddleware
    {
        public async Task Invoke(IMessageContext context, MiddlewareDelegate next)
        {
            using var activity = ActivitySourceAccessor.ActivitySource.StartActivity("integration-test", ActivityKind.Internal);

            MessageStorage.Add((byte[]) context.Message.Value);
            await next(context);
        }
    }
}
