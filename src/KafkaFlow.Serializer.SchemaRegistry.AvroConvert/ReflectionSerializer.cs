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
    private readonly bool _normalizeSchemas;
    private readonly bool _useLatestVersion;
    private readonly int _initialBufferSize;
    private readonly SubjectNameStrategyDelegate _subjectNameStrategy;

    private readonly string _schemaString;
    private readonly string _typeName;
    private readonly ConcurrentDictionary<string, Lazy<Task<int>>> _schemaIds = new();

    public ReflectionSerializer(ISchemaRegistryClient schemaRegistry, ISchemaStringTypeNameResolver typeNameResolver, AvroSerializerConfig config = null)
    {
        _schemaRegistry = schemaRegistry;

        _autoRegisterSchemas = config?.AutoRegisterSchemas ?? true;
        _normalizeSchemas = config?.NormalizeSchemas ?? false;
        _useLatestVersion = config?.UseLatestVersion ?? false;
        _initialBufferSize = config?.BufferBytes ?? 1024;

        _subjectNameStrategy = config?.SubjectNameStrategy != null
            ? config.SubjectNameStrategy.Value.ToDelegate()
            : SubjectNameStrategy.Topic.ToDelegate();

        if (_useLatestVersion && _autoRegisterSchemas)
        {
            throw new ArgumentException(
                "AvroConvertSerializer: Cannot enable both UseLatestVersion and AutoRegisterSchemas");
        }

        _schemaString = AvroConvert.GenerateSchema(typeof(T));
        _typeName = typeNameResolver.Resolve(_schemaString);
    }

    public async Task<byte[]> SerializeAsync(T data, SerializationContext context)
    {
        var subject = GetSubject(context);

        var schemaId = await _schemaIds.GetOrAdd(subject, x => new Lazy<Task<int>>(() => GetSchemaId(x))).Value.ConfigureAwait(false);

        return GetPayload(data, schemaId);
    }

    private string GetSubject(SerializationContext context)
    {
        var serializationContext = new SerializationContext(context.Component, context.Topic);

        return _subjectNameStrategy(serializationContext, _typeName);
    }

    private byte[] GetPayload(T data, int schemaId)
    {
        var schemaIdBytes = new byte[4];
        BinaryPrimitives.WriteInt32BigEndian(schemaIdBytes, schemaId);

        var serializedData = AvroConvert.SerializeHeadless(data, _schemaString);

        using var stream = new MemoryStream(_initialBufferSize);

        stream.WriteByte(0);
        stream.Write(schemaIdBytes, 0, schemaIdBytes.Length);
        stream.Write(serializedData, 0, serializedData.Length);

        return stream.ToArray();
    }

    private async Task<int> GetSchemaId(string subject)
    {
        if (_useLatestVersion)
        {
            var latestSchema = await _schemaRegistry.GetLatestSchemaAsync(subject).ConfigureAwait(false);

            return latestSchema.Id;
        }

        var schema = new Schema(_schemaString, SchemaType.Avro);

        return _autoRegisterSchemas
            ? await _schemaRegistry.RegisterSchemaAsync(subject, schema, _normalizeSchemas).ConfigureAwait(false)
            : await _schemaRegistry.GetSchemaIdAsync(subject, schema, _normalizeSchemas).ConfigureAwait(false);
    }
}
