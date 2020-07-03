namespace KafkaFlow
{
    using System;

    /// <summary>
    /// Represents the interface to be implemented by custom message serializers
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
