namespace KafkaFlow.Client.Protocol.Messages
{
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using KafkaFlow.Client.Protocol.Streams;

    public class Headers : ISerializable, IEnumerable<KeyValuePair<string, byte[]>>, IRequest, IResponse
    {
        private readonly ConcurrentDictionary<string, List<byte[]?>> values = new ConcurrentDictionary<string, List<byte[]?>>();

        public void Add(string key, byte[]? value)
        {
            this.values.GetOrAdd(key, new List<byte[]?>()).Add(value);
            ++this.Count;
        }

        public void Remove(string key)
        {
            if (this.values.TryRemove(key, out var v))
                this.Count -= v.Count;
        }

        public IReadOnlyList<byte[]?>? GetValues(string key)
        {
            return this.values.TryGetValue(key, out var values) ? values : null;
        }

        public byte[]? this[string key]
        {
            get => this.GetValues(key)?.FirstOrDefault();
            set
            {
                this.Remove(key);
                this.Add(key, value);
            }
        }

        public int Count { get; private set; }

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

        public void Write(Stream destination)
        {
            destination.WriteVarint(this.Count);

            foreach (var values in this.values)
            {
                var keyBytes = Encoding.UTF8.GetBytes(values.Key);

                foreach (var value in values.Value)
                {
                    destination.WriteVarint(keyBytes.Length);
                    destination.Write(keyBytes);

                    if (value is null)
                    {
                        destination.WriteVarint(-1);
                    }
                    else
                    {
                        destination.WriteVarint(value.Length);
                        destination.Write(value);
                    }
                }
            }
        }

        public void Read(Stream source)
        {
            var count = source.ReadVarint();

            for (var i = 0; i < count; ++i)
            {
                var key = source.ReadString(source.ReadVarint());
                var value = source.ReadBytes(source.ReadVarint());
                
                this.Add(key, value);
            }
        }
    }
}
