using System;
using System.Collections.Generic;
using KafkaFlow.Batching;
using KafkaFlow.Configuration;
using KafkaFlow.Consumers;

namespace KafkaFlow;

/// <summary>
/// no needed
/// </summary>
public static class BatchingExtensions
{
    /// <summary>
    /// Accumulates a group of messages to be passed as a batch to the next middleware as just one message
    /// </summary>
    /// <param name="builder">The configuration builder</param>
    /// <param name="batchSize">The maximum size of the batch, when this limit is reached the next middleware will be called</param>
    /// <param name="batchTimeout">The maximum time the middleware will wait to call the next middleware</param>
    /// <returns></returns>
    public static IConsumerMiddlewareConfigurationBuilder AddBatching(
        this IConsumerMiddlewareConfigurationBuilder builder,
        int batchSize,
        TimeSpan batchTimeout)
    {
        return builder.Add(
            resolver => new BatchConsumeMiddleware(
                resolver.Resolve<IConsumerMiddlewareContext>(),
                batchSize,
                batchTimeout,
                resolver.Resolve<ILogHandler>()),
            MiddlewareLifetime.Worker);
    }

    /// <summary>
    /// Gets the accumulated <see cref="IMessageContext"/> grouped by batching middleware
    /// </summary>
    /// <param name="context">The message context</param>
    /// <returns>All the contexts in the batch</returns>
    public static IReadOnlyCollection<IMessageContext> GetMessagesBatch(this IMessageContext context)
    {
        if (context is BatchConsumeMessageContext ctx)
        {
            return (IReadOnlyCollection<IMessageContext>)ctx.Message.Value;
        }

        throw new InvalidOperationException($"This method can only be used on {nameof(BatchConsumeMessageContext)}");
    }
}
