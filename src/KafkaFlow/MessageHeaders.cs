using System.Collections;
using System.Collections.Generic;
using Confluent.Kafka;

namespace KafkaFlow
{
    /// <summary>
    /// Collection of message headers
    /// </summary>
    public class MessageHeaders : IMessageHeaders
    {
        private readonly Headers _headers;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHeaders"/> class.
        /// </summary>
        /// <param name="headers">The Confluent headers</param>
        public MessageHeaders(Headers headers)
        {
            _headers = headers;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHeaders"/> class.
        /// </summary>
        public MessageHeaders()
            : this(new Headers())
        {
        }

        /// <summary>
        /// Gets the header with specified key
        /// </summary>
        /// <param name="key">The zero-based index of the element to get</param>
        public byte[] this[string key]
        {
            get => _headers.TryGetLastBytes(key, out var value) ? value : null;
            set
            {
                _headers.Remove(key);
                _headers.Add(key, value);
            }
        }

        /// <summary>
        /// Adds a new header to the enumeration
        /// </summary>
        /// <param name="key">The header key.</param>
        /// <param name="value">The header value (possibly null)</param>
        public void Add(string key, byte[] value)
        {
            _headers.Add(key, value);
        }

        /// <summary>
        /// Gets all the kafka headers
        /// </summary>
        /// <returns></returns>
        public Headers GetKafkaHeaders() => _headers;

        /// <summary>
        /// Gets an enumerator that iterates through <see cref="Headers"/>
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<string, byte[]>> GetEnumerator()
        {
            foreach (var header in _headers)
            {
                yield return new KeyValuePair<string, byte[]>(header.Key, header.GetValueBytes());
            }
        }

        /// <inheritdoc cref="GetEnumerator"/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
