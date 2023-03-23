using KafkaFlow.Consumers;

namespace KafkaFlow.Sample.PauseConsumerOnError;

public class PauseConsumerOnExceptionMiddleware : IMessageMiddleware
{
    private readonly IConsumerAccessor consumerAccessor;
    private readonly ILogHandler logHandler;

    public PauseConsumerOnExceptionMiddleware(IConsumerAccessor consumerAccessor, ILogHandler logHandler)
    {
        this.consumerAccessor = consumerAccessor;
        this.logHandler = logHandler;
    }

    public async Task Invoke(IMessageContext context, MiddlewareDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            context.ConsumerContext.ShouldStoreOffset = false;
            this.logHandler.Error("Error handling message", exception,
                new
                {
                    context.Message,
                    context.ConsumerContext.Topic,
                    MessageKey = context.Message.Key,
                    context.ConsumerContext.ConsumerName,
                });

            var consumer = this.consumerAccessor[context.ConsumerContext.ConsumerName];
            consumer.Pause(consumer.Assignment);

            this.logHandler.Warning("Consumer stopped", context.ConsumerContext.ConsumerName);
        }
    }
}