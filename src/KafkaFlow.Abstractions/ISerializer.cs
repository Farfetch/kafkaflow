namespace KafkaFlow
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Used to implement a message serializer
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        /// Serializes the given message
        /// </summary>
        /// <param name="message">The message to be serialized</param>
        /// <param name="output">A stream to write the serialized data</param>
        /// <param name="context">An object containing metadata</param>
        /// <returns>The serialized message</returns>
        Task SerializeAsync(object message, Stream output, ISerializerContext context);

        /// <summary>
        /// Deserializes the given message
        /// </summary>
        /// <param name="input">A stream to read the data to be deserialized</param>
        /// <param name="type">The type to be created</param>
        /// <param name="context">An object containing metadata</param>
        /// <returns>The deserialized message</returns>
        Task<object> DeserializeAsync(Stream input, Type type, ISerializerContext context);
    }
}
