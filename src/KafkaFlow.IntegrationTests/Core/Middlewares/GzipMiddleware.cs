namespace KafkaFlow.IntegrationTests.Core.Middlewares
{
    using System.Threading.Tasks;
    using Handlers;

    public class GzipMiddleware : IMessageMiddleware
    {
      public async Task Invoke(IMessageContext context, MiddlewareDelegate next)
        {
            MessageStorage.Add((byte[]) context.Message);        
            await next(context);
        }
    }
}