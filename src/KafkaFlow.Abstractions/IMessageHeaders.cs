namespace KafkaFlow
{
    using System.Collections.Generic;

    public interface IMessageHeaders : IEnumerable<KeyValuePair<string, byte[]>>
    {
        void Add(string key, byte[] value);

        byte[] this[string key] { get; set; }
    }
}
