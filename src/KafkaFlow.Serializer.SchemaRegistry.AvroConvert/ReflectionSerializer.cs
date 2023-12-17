using System;
using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using SolTechnology.Avro;

namespace KafkaFlow.Serializer.SchemaRegistry;

internal class ReflectionSerializer<T> : IAsyncSerializer<T>
{
    private readonly ISchemaRegistryClient _schemaRegistry;
    private readonly bool _autoRegisterSchemas;
    private readonly ConcurrentDictionary<string, Lazy<Task<int>>> _schemaIds = new();
    private readonly Lazy<string> _schema = new(() => AvroConvert.GenerateSchema(typeof(T)));

    public ReflectionSerializer(ISchemaRegistryClient schemaRegistry, AvroSerializerConfig config = null)
    {
        _schemaRegistry = schemaRegistry;
        _autoRegisterSchemas = config?.AutoRegisterSchemas ?? true;
    }

    public async Task<byte[]> SerializeAsync(T data, SerializationContext context)
    {
        var subject = $"{context.Topic}-{context.Component.ToString().ToLower()}";

        var schemaId = await _schemaIds.GetOrAdd(subject, x => new Lazy<Task<int>>(() => GetSchemaId(x))).Value;

        return GetPayload(data, schemaId);
    }

    private byte[] GetPayload(T data, int schemaId)
    {
        var schemaIdBytes = new byte[4];
        BinaryPrimitives.WriteInt32BigEndian(schemaIdBytes, schemaId);

        var serializedData = AvroConvert.SerializeHeadless(data, _schema.Value);

        using var stream = new MemoryStream(1024);

        stream.WriteByte(0);
        stream.Write(schemaIdBytes, 0, schemaIdBytes.Length);
        stream.Write(serializedData, 0, serializedData.Length);

        return stream.ToArray();
    }

    private async Task<int> GetSchemaId(string subject)
    {
        var schema = new Schema(_schema.Value, SchemaType.Avro);

        if (_autoRegisterSchemas)
        {
            return await _schemaRegistry.RegisterSchemaAsync(subject, schema).ConfigureAwait(false);
        }

        return await _schemaRegistry.GetSchemaIdAsync(subject, schema).ConfigureAwait(false);
    }
}
