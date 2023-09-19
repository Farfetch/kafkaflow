namespace KafkaFlow.Middlewares.TypedHandler
{
    using System;
    using System.Collections.Generic;

    internal class HandlerTypeMapping
    {
        private static readonly IReadOnlyList<Type> EmptyList = new List<Type>().AsReadOnly();

        private readonly Dictionary<Type, List<Type>> mapping = new();

        public void AddMapping(Type messageType, Type handlerType)
        {
            if (!this.mapping.TryGetValue(messageType, out var handlers))
            {
                handlers = new List<Type>();
                this.mapping.Add(messageType, handlers);
            }

            handlers.Add(handlerType);
        }

        public IReadOnlyList<Type> GetHandlersTypes(Type messageType)
        {
            if (messageType is null)
            {
                return EmptyList;
            }

            return this.mapping.TryGetValue(messageType, out var handlerType) ?
                handlerType :
                EmptyList;
        }
    }
}
