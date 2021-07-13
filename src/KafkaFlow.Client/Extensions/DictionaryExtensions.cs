namespace KafkaFlow.Client.Extensions
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    internal static class DictionaryExtensions
    {
        public static TValue ThreadSafeGetOrAdd<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary,
            TKey key,
            Func<TKey, TValue> factory)
        {
            if (dictionary.TryGetValue(key, out var value))
            {
                return value;
            }

            lock (dictionary)
            {
                if (dictionary.TryGetValue(key, out value))
                {
                    return value;
                }

                value = factory(key);

                dictionary.Add(key, value);
            }

            return value;
        }

        public static TValue GetOrAdd<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary,
            TKey key,
            Func<TKey, TValue> factory)
        {
            if (dictionary.TryGetValue(key, out var value))
            {
                return value;
            }

            value = factory(key);

            dictionary.Add(key, value);

            return value;
        }
    }
}
