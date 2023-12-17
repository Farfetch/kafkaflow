using System;
using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using SolTechnology.Avro;

namespace KafkaFlow.Serializer.SchemaRegistry;

internal class ReflectionDeserializer<T> : IAsyncDeserializer<T>
{
    private readonly ISchemaRegistryClient _schemaRegistry;
    private readonly ConcurrentDictionary<int, Lazy<Task<Schema>>> _cache = new();

    public ReflectionDeserializer(ISchemaRegistryClient schemaRegistry)
    {
        _schemaRegistry = schemaRegistry;
    }

    public async Task<T> DeserializeAsync(ReadOnlyMemory<byte> data, bool isNull, SerializationContext context)
    {
        if (data.Length < 5)
        {
            throw new InvalidDataException($"Expecting data framing of length 5 bytes or more but total data size is {data.Length} bytes");
        }

        var header = data.Slice(0, 1);
        var schemaId = BinaryPrimitives.ReadInt32BigEndian(data.Span.Slice(1, 4));
        var payload = data.Slice(5);

        if (header.Span[0] != 0)
        {
            throw new InvalidDataException($"Expecting data with Confluent Schema Registry framing. Magic byte was {header.Span[0]}, expecting {0}");
        }

        if (_cache.Count > _schemaRegistry.MaxCachedSchemas)
        {
            _cache.Clear();
        }

        var schema = await _cache.GetOrAdd(schemaId, key => new Lazy<Task<Schema>>(() => _schemaRegistry.GetSchemaAsync(key))).Value;

        return AvroConvert.DeserializeHeadless<T>(payload.ToArray(), schema.SchemaString);
    }
}
