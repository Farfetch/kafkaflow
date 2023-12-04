using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace KafkaFlow.Serializer;

/// <summary>
/// A message serializer using NewtonsoftJson library
/// </summary>
public class NewtonsoftJsonSerializer : ISerializer
{
    private const int DefaultBufferSize = 1024;

    private static readonly UTF8Encoding s_uTF8NoBom = new(false);

    private readonly JsonSerializerSettings _settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="NewtonsoftJsonSerializer"/> class.
    /// </summary>
    /// <param name="settings">Json serializer settings</param>
    public NewtonsoftJsonSerializer(JsonSerializerSettings settings)
    {
        _settings = settings;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NewtonsoftJsonSerializer"/> class.
    /// </summary>
    public NewtonsoftJsonSerializer()
        : this(new JsonSerializerSettings())
    {
    }

    /// <inheritdoc/>
    public Task SerializeAsync(object message, Stream output, ISerializerContext context)
    {
        using var sw = new StreamWriter(output, s_uTF8NoBom, DefaultBufferSize, true);
        var serializer = JsonSerializer.CreateDefault(_settings);

        serializer.Serialize(sw, message);

        return Task.CompletedTask;
    }
}
