using System.Diagnostics;
using System.Threading.Tasks;
using KafkaFlow.IntegrationTests.Core.Handlers;
using KafkaFlow.OpenTelemetry;

namespace KafkaFlow.IntegrationTests.Core.Middlewares
{
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