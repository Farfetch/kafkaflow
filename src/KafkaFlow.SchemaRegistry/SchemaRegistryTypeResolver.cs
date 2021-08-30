namespace KafkaFlow
{
    using System;
    using System.Buffers.Binary;
    using System.Collections.Concurrent;
    using System.Linq;

    /// <summary>
    ///  The message type resolver to be used with schema registry serializers
    /// </summary>
    public class SchemaRegistryTypeResolver : IMessageTypeResolver
    {
        private static readonly ConcurrentDictionary<int, Type> Types = new();

        private readonly ISchemaRegistryTypeNameResolver typeNameResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="SchemaRegistryTypeResolver"/> class.
        /// </summary>
        /// <param name="typeNameResolver">A instance of the <see cref="ISchemaRegistryTypeNameResolver"/> interface.</param>
        public SchemaRegistryTypeResolver(ISchemaRegistryTypeNameResolver typeNameResolver)
        {
            this.typeNameResolver = typeNameResolver;
        }

        /// <inheritdoc />
        public Type OnConsume(IMessageContext context)
        {
            var schemaId = BinaryPrimitives.ReadInt32BigEndian(
                ((byte[]) context.Message.Value).AsSpan().Slice(1, 4));

            return Types.GetOrAdd(
                schemaId,
                id =>
                {
                    var typeName = this.typeNameResolver.Resolve(id);

                    return AppDomain.CurrentDomain
                        .GetAssemblies()
                        .Select(a => a.GetType(typeName))
                        .FirstOrDefault(x => x != null);
                });
        }

        /// <inheritdoc />
        public void OnProduce(IMessageContext context)
        {
            // Do nothing
        }
    }
}
