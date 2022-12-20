namespace KafkaFlow
{
    using System;
    using System.Buffers.Binary;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    ///  The message type resolver to be used with schema registry serializers
    /// </summary>
    public class SchemaRegistryTypeResolver : IAsyncMessageTypeResolver
    {
        private static readonly ConcurrentDictionary<int, Type> Types = new();

        private static readonly SemaphoreSlim Semaphore = new(1, 1);

        private readonly IAsyncSchemaRegistryTypeNameResolver typeNameResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="SchemaRegistryTypeResolver"/> class.
        /// </summary>
        /// <param name="typeNameResolver">A instance of the <see cref="ISchemaRegistryTypeNameResolver"/> interface.</param>
        public SchemaRegistryTypeResolver(ISchemaRegistryTypeNameResolver typeNameResolver)
            : this(new AsyncSchemaRegistryTypeNameResolverWrapper(typeNameResolver))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SchemaRegistryTypeResolver"/> class.
        /// </summary>
        /// <param name="typeNameResolver">A instance of the <see cref="ISchemaRegistryTypeNameResolver"/> interface.</param>
        public SchemaRegistryTypeResolver(IAsyncSchemaRegistryTypeNameResolver typeNameResolver)
        {
            this.typeNameResolver = typeNameResolver;
        }

        /// <inheritdoc />
        public async Task<Type> OnConsumeAsync(IMessageContext context)
        {
            var schemaId = BinaryPrimitives.ReadInt32BigEndian(
                ((byte[]) context.Message.Value).AsSpan().Slice(1, 4));

            if (Types.TryGetValue(schemaId, out var type))
            {
                return type;
            }

            await Semaphore.WaitAsync();

            try
            {
                if (Types.TryGetValue(schemaId, out type))
                {
                    return type;
                }

                var typeName = await this.typeNameResolver.ResolveAsync(schemaId);

                return Types[schemaId] = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .Select(a => a.GetType(typeName))
                    .FirstOrDefault(x => x != null);
            }
            finally
            {
                Semaphore.Release();
            }
        }

        /// <inheritdoc />
        public Task OnProduceAsync(IMessageContext context) => Task.CompletedTask;
    }
}
