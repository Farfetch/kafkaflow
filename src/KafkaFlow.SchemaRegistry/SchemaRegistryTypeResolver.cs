using System;
using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KafkaFlow.Middlewares.Serializer.Resolvers;

namespace KafkaFlow
{
    /// <summary>
    ///  The message type resolver to be used with schema registry serializers
    /// </summary>
    public class SchemaRegistryTypeResolver : IMessageTypeResolver
    {
        private static readonly ConcurrentDictionary<int, Type> s_types = new();

        private static readonly SemaphoreSlim s_semaphore = new(1, 1);

        private readonly ISchemaRegistryTypeNameResolver _typeNameResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="SchemaRegistryTypeResolver"/> class.
        /// </summary>
        /// <param name="typeNameResolver">A instance of the <see cref="ISchemaRegistryTypeNameResolver"/> interface.</param>
        public SchemaRegistryTypeResolver(ISchemaRegistryTypeNameResolver typeNameResolver)
        {
            _typeNameResolver = typeNameResolver;
        }

        /// <inheritdoc />
        public async ValueTask<Type> OnConsumeAsync(IMessageContext context)
        {
            var schemaId = BinaryPrimitives.ReadInt32BigEndian(
                ((byte[])context.Message.Value).AsSpan().Slice(1, 4));

            if (s_types.TryGetValue(schemaId, out var type))
            {
                return type;
            }

            await s_semaphore.WaitAsync();

            try
            {
                if (s_types.TryGetValue(schemaId, out type))
                {
                    return type;
                }

                var typeName = await _typeNameResolver.ResolveAsync(schemaId);

                return s_types[schemaId] = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .Select(a => a.GetType(typeName))
                    .FirstOrDefault(x => x != null);
            }
            finally
            {
                s_semaphore.Release();
            }
        }

        /// <inheritdoc />
        public ValueTask OnProduceAsync(IMessageContext context) => default(ValueTask);
    }
}
