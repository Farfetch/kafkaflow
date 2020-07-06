namespace KafkaFlow
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a collection of message headers
    /// </summary>
    public interface IMessageHeaders : IEnumerable<KeyValuePair<string, byte[]>>
    {
        /// <summary>
        /// Adds a new header to the enumeration
        /// </summary>
        /// <param name="key">The header key.</param>
        /// <param name="value">The header value (possibly null)</param>
        void Add(string key, byte[] value);

        /// <summary>
        /// Gets the header with specified key
        /// </summary>
        /// <param name="key">The zero-based index of the element to get</param>
        byte[] this[string key] { get; set; }
    }
}
