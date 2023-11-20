namespace KafkaFlow.IntegrationTests.Core.Middlewares
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using KafkaFlow.IntegrationTests.Core.Handlers;
    using KafkaFlow.OpenTelemetry;

    internal class GzipMiddleware : IMessageMiddleware
    {
        public async Task Invoke(IMessageContext context, MiddlewareDelegate next)
        {
            var source = new ActivitySource(KafkaFlowInstrumentation.ActivitySourceName);
            using var activity = source.StartActivity("integration-test", ActivityKind.Internal);

            MessageStorage.Add((byte[])context.Message.Value);
            await next(context);
        }
    }
}
