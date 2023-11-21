using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace KafkaFlow.Serializer
{
    /// <summary>
    /// A message deserializer using System.Text.Json library
    /// </summary>
    public class JsonCoreDeserializer : IDeserializer
    {
        private readonly JsonSerializerOptions _serializerOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonCoreDeserializer"/> class.
        /// </summary>
        /// <param name="options">Json serializer options</param>
        public JsonCoreDeserializer(JsonSerializerOptions options)
        {
            _serializerOptions = options;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonCoreDeserializer"/> class.
        /// </summary>
        public JsonCoreDeserializer()
            : this(new JsonSerializerOptions())
        {
        }

        /// <inheritdoc/>
        public async Task<object> DeserializeAsync(Stream input, Type type, ISerializerContext context)
        {
            return await JsonSerializer
                .DeserializeAsync(input, type, _serializerOptions)
                .ConfigureAwait(false);
        }
    }
}
