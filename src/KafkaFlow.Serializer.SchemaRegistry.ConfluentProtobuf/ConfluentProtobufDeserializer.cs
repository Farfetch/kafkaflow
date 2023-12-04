using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Confluent.SchemaRegistry.Serdes;

namespace KafkaFlow.Serializer.SchemaRegistry;

/// <summary>
/// A protobuf message serializer integrated with the confluent schema registry
/// </summary>
public class ConfluentProtobufDeserializer : IDeserializer
{
    /// <inheritdoc/>
    public Task<object> DeserializeAsync(Stream input, Type type, ISerializerContext context)
    {
        return ConfluentDeserializerWrapper
            .GetOrCreateDeserializer(
                type,
                () => Activator
                    .CreateInstance(
                        typeof(ProtobufDeserializer<>).MakeGenericType(type),
                        (IEnumerable<KeyValuePair<string, string>>)null))
            .DeserializeAsync(input, context);
    }
}
