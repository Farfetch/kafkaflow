namespace KafkaFlow
{
    using System;

    /// <summary>
    /// Used to implement a message serializer
    /// </summary>
    public interface IMessageSerializer
    {
        /// <summary>
        /// Serializes the given message
        /// </summary>
        /// <param name="message">The message to be serialized</param>
        /// <returns>The serialized message</returns>
        byte[] Serialize(object message);

        /// <summary>
        /// Deserializes the given message
        /// </summary>
        /// <param name="message">The message to be deserialized</param>
        /// <param name="type">The type to be created</param>
        /// <returns>The deserialized message</returns>
        object Deserialize(byte[] message, Type type);
    }
}
