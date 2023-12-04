using System.IO;
using System.Threading.Tasks;

namespace KafkaFlow;

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
}
