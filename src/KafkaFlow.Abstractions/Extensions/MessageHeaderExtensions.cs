namespace KafkaFlow
{
    using System.Text;

    /// <summary>
    /// Provides extension methods over <see cref="IMessageHeaders"/>
    /// </summary>
    public static class MessageHeaderExtensions
    {
        /// <summary>
        /// Gets a header value as string
        /// </summary>
        /// <param name="headers">The <see cref="IMessageHeaders"/> object that this method was called on.</param>
        /// <param name="key">The header key</param>
        /// <param name="encoding">The encoding used to get the string</param>
        /// <returns>The retrieved string header value</returns>
        public static string GetString(this IMessageHeaders headers, string key, Encoding encoding)
        {
            var value = headers[key];
            return value != null ? encoding.GetString(headers[key]) : null;
        }

        /// <summary>
        /// Gets a header value as an UTF8 string
        /// </summary>
        /// <param name="headers">The <see cref="IMessageHeaders"/> object that this method was called on.</param>
        /// <param name="key">The header key</param>
        /// <returns>The retrieved string header value</returns>
        public static string GetString(this IMessageHeaders headers, string key) =>
            headers.GetString(key, Encoding.UTF8);

        /// <summary>
        /// Sets a header value as string
        /// </summary>
        /// <param name="headers">The <see cref="IMessageHeaders"/> object that this method was called on.</param>
        /// <param name="key">The header key</param>
        /// <param name="value">The header value</param>
        /// <param name="encoding">The encoding used to store the string</param>
        public static void SetString(
            this IMessageHeaders headers,
            string key,
            string value,
            Encoding encoding)
        {
            headers[key] = encoding.GetBytes(value);
        }

        /// <summary>
        /// Sets a header value as UTF8 string
        /// </summary>
        /// <param name="headers">The <see cref="IMessageHeaders"/> object that this method was called on.</param>
        /// <param name="key">The header key</param>
        /// <param name="value">The header value</param>
        public static void SetString(
            this IMessageHeaders headers,
            string key,
            string value) =>
            headers.SetString(
                key,
                value,
                Encoding.UTF8);
    }
}
