using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace KafkaFlow.Serializer;

/// <summary>
/// A message deserializer using NewtonsoftJson library
/// </summary>
public class NewtonsoftJsonDeserializer : IDeserializer
{
    private const int DefaultBufferSize = 1024;

    private static readonly UTF8Encoding s_uTF8NoBom = new(false);

    private readonly JsonSerializerSettings _settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="NewtonsoftJsonDeserializer"/> class.
    /// </summary>
    /// <param name="settings">Json serializer settings</param>
    public NewtonsoftJsonDeserializer(JsonSerializerSettings settings)
    {
        _settings = settings;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NewtonsoftJsonDeserializer"/> class.
    /// </summary>
    public NewtonsoftJsonDeserializer()
        : this(new JsonSerializerSettings())
    {
    }

    /// <inheritdoc/>
    public Task<object> DeserializeAsync(Stream input, Type type, ISerializerContext context)
    {
        using var sr = new StreamReader(
            input,
            s_uTF8NoBom,
            true,
            DefaultBufferSize,
            true);

        var serializer = JsonSerializer.CreateDefault(_settings);

        return Task.FromResult(serializer.Deserialize(sr, type));
    }
}
