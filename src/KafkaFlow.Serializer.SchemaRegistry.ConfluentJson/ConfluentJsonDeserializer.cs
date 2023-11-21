using System;
using System.IO;
using System.Threading.Tasks;
using Confluent.SchemaRegistry.Serdes;
using NJsonSchema.Generation;

namespace KafkaFlow.Serializer.SchemaRegistry
{
    /// <summary>
    /// A json message serializer integrated with the confluent schema registry
    /// </summary>
    public class ConfluentJsonDeserializer : IDeserializer
    {
        private readonly JsonSchemaGeneratorSettings _schemaGeneratorSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfluentJsonDeserializer"/> class.
        /// </summary>
        /// <param name="schemaGeneratorSettings">An instance of <see cref="JsonSchemaGeneratorSettings"/></param>
        public ConfluentJsonDeserializer(JsonSchemaGeneratorSettings schemaGeneratorSettings = null)
        {
            _schemaGeneratorSettings = schemaGeneratorSettings;
        }

        /// <inheritdoc/>
        public Task<object> DeserializeAsync(Stream input, Type type, ISerializerContext context)
        {
            return ConfluentDeserializerWrapper
                .GetOrCreateDeserializer(
                    type,
                    () => Activator
                        .CreateInstance(
                            typeof(JsonDeserializer<>).MakeGenericType(type),
                            null,
                            _schemaGeneratorSettings))
                .DeserializeAsync(input, context);
        }
    }
}
