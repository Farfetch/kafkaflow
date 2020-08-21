namespace KafkaFlow.Client.Core
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;

    internal class ConcurrentAsyncDictionary<TKey, TValue> : ConcurrentDictionary<TKey, TValue>
    {
        private readonly SemaphoreSlim addSemaphore = new SemaphoreSlim(1, 1);

        public async ValueTask<TValue> GetOrAddAsync(TKey key, Func<Task<TValue>> addHandler)
        {
            if (this.TryGetValue(key, out var value))
                return value;

            await this.addSemaphore.WaitAsync().ConfigureAwait(false);

            try
            {
                if (this.TryGetValue(key, out value))
                    return value;

                value = await addHandler().ConfigureAwait(false);

                this.TryAdd(key, value);
            }
            finally
            {
                this.addSemaphore.Release();
            }

            return value;
        }
    }
}