using System;
using System.IO;
using System.Threading.Tasks;

namespace KafkaFlow;

/// <summary>
/// Used to implement a message serializer
/// </summary>
public interface IDeserializer
{
    /// <summary>
    /// Deserializes the given message
    /// </summary>
    /// <param name="input">A stream to read the data to be deserialized</param>
    /// <param name="type">The type to be created</param>
    /// <param name="context">An object containing metadata</param>
    /// <returns>The deserialized message</returns>
    Task<object> DeserializeAsync(Stream input, Type type, ISerializerContext context);
}
