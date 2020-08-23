namespace KafkaFlow.Client.Extensions
{
    using System;
    using System.Collections.Concurrent;

    internal static class ConcurrentDictionaryExtensions
    {
        public static TValue SafeGetOrAdd<TKey, TValue>(
            this ConcurrentDictionary<TKey, TValue> dictionary,
            TKey key,
            Func<TKey, TValue> factory)
        {
            if (dictionary.TryGetValue(key, out var value))
                return value;

            lock (dictionary)
            {
                if (dictionary.TryGetValue(key, out value))
                    return value;

                value = factory(key);

                dictionary.TryAdd(key, value);
            }

            return value;
        }
    }
}
