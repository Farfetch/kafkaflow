using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace KafkaFlow.Serializer;

/// <summary>
/// A message serializer using System.Text.Json library
/// </summary>
public class JsonCoreSerializer : ISerializer
{
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly JsonWriterOptions _writerOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonCoreSerializer"/> class.
    /// </summary>
    /// <param name="options">Json serializer options</param>
    public JsonCoreSerializer(JsonSerializerOptions options)
    {
        _serializerOptions = options;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonCoreSerializer"/> class.
    /// </summary>
    /// <param name="writerOptions">Json writer options</param>
    public JsonCoreSerializer(JsonWriterOptions writerOptions)
    {
        _writerOptions = writerOptions;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonCoreSerializer"/> class.
    /// </summary>
    /// <param name="serializerOptions">Json serializer options</param>
    /// <param name="writerOptions">Json writer options</param>
    public JsonCoreSerializer(JsonSerializerOptions serializerOptions, JsonWriterOptions writerOptions)
    {
        _serializerOptions = serializerOptions;
        _writerOptions = writerOptions;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonCoreSerializer"/> class.
    /// </summary>
    public JsonCoreSerializer()
        : this(new JsonSerializerOptions(), default)
    {
    }

    /// <inheritdoc/>
    public Task SerializeAsync(object message, Stream output, ISerializerContext context)
    {
        if (message is null || message == Array.Empty<byte>())
        {
            return Task.CompletedTask;
        }

        using var writer = new Utf8JsonWriter(output, _writerOptions);

        JsonSerializer.Serialize(writer, message, _serializerOptions);

        return Task.CompletedTask;
    }
}
