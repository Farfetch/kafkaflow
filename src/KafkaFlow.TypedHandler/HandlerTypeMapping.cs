namespace KafkaFlow.TypedHandler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class HandlerTypeMapping
    {
        private readonly Dictionary<Type, IEnumerable<Type>> mapping = new Dictionary<Type, IEnumerable<Type>>();

        public void AddMapping(Type messageType, Type handlerType)
        {
            var handlers = this.GetHandlersTypes(messageType);
            
            this.mapping[messageType] = handlers.Append(handlerType);
        }

        public IEnumerable<Type> GetHandlersTypes(Type messageType)
        {
            return this.mapping.TryGetValue(messageType, out var handlerType) ? handlerType : Enumerable.Empty<Type>();
        }
    }
}
