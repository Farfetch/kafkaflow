namespace KafkaFlow.Client
{
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;

    public class Headers : IHeaders
    {
        private readonly ConcurrentDictionary<string, List<byte[]>> values = new ConcurrentDictionary<string, List<byte[]>>();

        public void Add(string key, byte[] value)
        {
            this.values.GetOrAdd(key, new List<byte[]>()).Add(value);
        }

        public bool Remove(string key)
        {
            return this.values.TryRemove(key, out _);
        }

        public IReadOnlyList<byte[]>? GetValues(string key)
        {
            return this.values.TryGetValue(key, out var values) ? values : null;
        }

        public byte[]? this[string key]
        {
            get => this.GetValues(key)?.FirstOrDefault();
            set
            {
                var v = new List<byte[]> { value! };
                this.values.AddOrUpdate(key, v, (a, b) => v);
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            foreach (var (key, value) in this)
            {
                info.AddValue(key, Encoding.UTF8.GetString(value));
            }
        }

        public IEnumerator<KeyValuePair<string, byte[]>> GetEnumerator()
        {
            return this.values
                .SelectMany(x => x.Value.Select(y => new KeyValuePair<string, byte[]>(x.Key, y)))
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
