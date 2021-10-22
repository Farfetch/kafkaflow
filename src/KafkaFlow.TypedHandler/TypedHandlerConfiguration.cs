namespace KafkaFlow.TypedHandler
{
    using System;

    internal class TypedHandlerConfiguration
    {
        public HandlerTypeMapping HandlerMapping { get; } = new();

        public Action<IMessageContext> OnNoHandlerFound { get; set; } = (_) => { };
    }
}
