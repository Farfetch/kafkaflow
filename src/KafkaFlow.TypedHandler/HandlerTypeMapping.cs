namespace KafkaFlow.TypedHandler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class HandlerTypeMapping
    {
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

        public IEnumerable<Type> GetHandlersTypes(Type messageType)
        {
            return this.mapping.TryGetValue(messageType, out var handlerType) ?
                handlerType :
                Enumerable.Empty<Type>();
        }
    }
}
