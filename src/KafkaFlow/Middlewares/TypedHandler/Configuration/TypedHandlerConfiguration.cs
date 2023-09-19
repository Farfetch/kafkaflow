namespace KafkaFlow.Middlewares.TypedHandler.Configuration
{
    using System;

    internal class TypedHandlerConfiguration
    {
        public HandlerTypeMapping HandlerMapping { get; } = new();

        public Action<IMessageContext> OnNoHandlerFound { get; set; } = (_) => { };
    }
}
