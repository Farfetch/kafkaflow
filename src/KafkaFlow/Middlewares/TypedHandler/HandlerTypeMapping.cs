using System;
using System.Collections.Generic;

namespace KafkaFlow.Middlewares.TypedHandler
{
    internal class HandlerTypeMapping
    {
        private static readonly IReadOnlyList<Type> s_emptyList = new List<Type>().AsReadOnly();

        private readonly Dictionary<Type, List<Type>> _mapping = new();

        public void AddMapping(Type messageType, Type handlerType)
        {
            if (!_mapping.TryGetValue(messageType, out var handlers))
            {
                handlers = new List<Type>();
                _mapping.Add(messageType, handlers);
            }

            handlers.Add(handlerType);
        }

        public IReadOnlyList<Type> GetHandlersTypes(Type messageType)
        {
            if (messageType is null)
            {
                return s_emptyList;
            }

            return _mapping.TryGetValue(messageType, out var handlerType) ?
                handlerType :
                s_emptyList;
        }
    }
}
