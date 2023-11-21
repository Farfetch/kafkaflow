using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace KafkaFlow.Middlewares.TypedHandler
{
    internal abstract class HandlerExecutor
    {
        private static readonly ConcurrentDictionary<Type, HandlerExecutor> s_executors = new();

        public static HandlerExecutor GetExecutor(Type messageType)
        {
            return s_executors.SafeGetOrAdd(
                messageType,
                _ => (HandlerExecutor)Activator.CreateInstance(typeof(InnerHandlerExecutor<>).MakeGenericType(messageType)));
        }

        public abstract Task Execute(object handler, IMessageContext context, object message);

        private class InnerHandlerExecutor<T> : HandlerExecutor
        {
            public override Task Execute(object handler, IMessageContext context, object message)
            {
                var h = (IMessageHandler<T>)handler;

                return h.Handle(context, (T)message);
            }
        }
    }
}
