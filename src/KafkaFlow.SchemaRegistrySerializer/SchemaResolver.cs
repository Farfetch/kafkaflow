namespace KafkaFlow.SchemaRegistrySerializer
{
    using System;
    using System.Collections.Concurrent;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;

    internal class SchemaResolver : ISchemaResolver
    {
        private readonly ISchemaRegistryManager schemaRegistryManager;

        private readonly ConcurrentDictionary<Type, IRegisteredSchema> schemaCache =
            new ConcurrentDictionary<Type, IRegisteredSchema>();

        public SchemaResolver(ISchemaRegistryManager schemaRegistryManager)
        {
            this.schemaRegistryManager = schemaRegistryManager;
        }

        public async ValueTask<IRegisteredSchema> ResolveAsync(Type messageType)
        {
            if (this.schemaCache.TryGetValue(messageType, out var schema))
            {
                return schema;
            }

            var (subject, version) = GetSchemaInfo(messageType);

            schema = await this.schemaRegistryManager
                .GetSchemaAsync(subject, version)
                .ConfigureAwait(false);

            return this.schemaCache.GetOrAdd(messageType, schema);
        }

        private static (string, int) GetSchemaInfo(Type messageType)
        {
            return messageType.GetCustomAttribute(typeof(DataContractAttribute)) is DataContractAttribute dataContract ?
                ContractNameParser.GetSchemaData(dataContract.Name) :
                throw new InvalidDataContractException($"The type {messageType.FullName} must have a {nameof(DataContractAttribute)}");
        }
    }
}
