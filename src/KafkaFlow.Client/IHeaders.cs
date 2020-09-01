namespace KafkaFlow.Client
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    public interface IHeaders : ISerializable, IEnumerable<KeyValuePair<string, byte[]>>
    {
        void Add(string key, byte[] value);

        bool Remove(string key);

        IReadOnlyList<byte[]>? GetValues(string key);

        byte[]? this[string key] { get; set; }
    }
}
