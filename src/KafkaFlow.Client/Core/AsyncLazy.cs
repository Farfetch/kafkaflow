namespace KafkaFlow.Client.Core
{
    using System;
    using System.Threading.Tasks;

    public class AsyncLazy<T> : Lazy<ValueTask<T>>
    {
        public AsyncLazy(Func<ValueTask<T>> factory) : base(factory)
        {
        }
    }
}
