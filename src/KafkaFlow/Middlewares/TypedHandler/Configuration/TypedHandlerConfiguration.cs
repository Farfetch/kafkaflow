using System;

namespace KafkaFlow.Middlewares.TypedHandler.Configuration;

internal class TypedHandlerConfiguration
{
    public HandlerTypeMapping HandlerMapping { get; } = new();

    public Action<IMessageContext> OnNoHandlerFound { get; set; } = (_) => { };
}
