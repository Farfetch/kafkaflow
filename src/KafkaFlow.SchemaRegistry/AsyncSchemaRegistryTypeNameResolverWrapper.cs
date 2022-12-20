namespace KafkaFlow
{
    using System.Threading.Tasks;

    internal class AsyncSchemaRegistryTypeNameResolverWrapper : IAsyncSchemaRegistryTypeNameResolver
    {
        private readonly ISchemaRegistryTypeNameResolver typeNameResolver;

        public AsyncSchemaRegistryTypeNameResolverWrapper(ISchemaRegistryTypeNameResolver typeNameResolver) =>
            this.typeNameResolver = typeNameResolver;

        public Task<string> ResolveAsync(int schemaId) => Task.FromResult(this.typeNameResolver.Resolve(schemaId));
    }
}
