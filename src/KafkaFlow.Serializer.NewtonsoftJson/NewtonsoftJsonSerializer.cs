namespace KafkaFlow.Serializer
{
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    /// <summary>
    /// A message serializer using NewtonsoftJson library
    /// </summary>
    public class NewtonsoftJsonSerializer : ISerializer
    {
        private const int DefaultBufferSize = 1024;

        private static readonly UTF8Encoding UTF8NoBom = new (false);
        private readonly JsonSerializerSettings settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="NewtonsoftJsonSerializer"/> class.
        /// </summary>
        /// <param name="settings">Json serializer settings</param>
        public NewtonsoftJsonSerializer(JsonSerializerSettings settings)
        {
            this.settings = settings;
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
            using var sw = new StreamWriter(output, UTF8NoBom, DefaultBufferSize, true);
            var serializer = JsonSerializer.CreateDefault(this.settings);

            serializer.Serialize(sw, message);

            return Task.CompletedTask;
        }
    }
}
