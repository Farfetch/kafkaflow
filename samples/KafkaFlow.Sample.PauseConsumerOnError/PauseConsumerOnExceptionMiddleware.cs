using KafkaFlow.Consumers;

namespace KafkaFlow.Sample.PauseConsumerOnError;

public class PauseConsumerOnExceptionMiddleware : IMessageMiddleware
{
    private readonly IConsumerAccessor _consumerAccessor;
    private readonly ILogHandler _logHandler;

    public PauseConsumerOnExceptionMiddleware(IConsumerAccessor consumerAccessor, ILogHandler logHandler)
    {
        _consumerAccessor = consumerAccessor;
        _logHandler = logHandler;
    }

    public async Task Invoke(IMessageContext context, MiddlewareDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            context.ConsumerContext.AutoMessageCompletion = false;
            _logHandler.Error("Error handling message", exception,
                new
                {
                    context.Message,
                    context.ConsumerContext.Topic,
                    MessageKey = context.Message.Key,
                    context.ConsumerContext.ConsumerName,
                });

            var consumer = _consumerAccessor[context.ConsumerContext.ConsumerName];
            consumer.Pause(consumer.Assignment);

            _logHandler.Warning("Consumer stopped", context.ConsumerContext.ConsumerName);
        }
    }
}