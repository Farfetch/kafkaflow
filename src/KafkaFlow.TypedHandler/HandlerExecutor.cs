namespace KafkaFlow.TypedHandler
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;

    public abstract class HandlerExecutor
    {
        private static readonly ConcurrentDictionary<Type, HandlerExecutor> executors =
            new ConcurrentDictionary<Type, HandlerExecutor>();

        public static HandlerExecutor GetExecutor(Type messageType)
        {
            return executors.GetOrAdd(
                messageType,
                t => (HandlerExecutor)Activator.CreateInstance(typeof(HandlerExecutor<>).MakeGenericType(messageType)));
        }

        public abstract Task Execute(object handler, IMessageContext context, object message);
    }

    public class HandlerExecutor<T> : HandlerExecutor
    {
        public override Task Execute(object handler, IMessageContext context, object message)
        {
            var h = (IMessageHandler<T>)handler;

            return h.Handle(context, (T)message);
        }
    }
}
