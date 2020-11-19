namespace KafkaFlow.SchemaRegistrySerializer
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.SchemaRegistry;

    internal class SchemaRegistryManager : ISchemaRegistryManager, IDisposable
    {
        private readonly ISchemaRegistryClient registryClient;

        private readonly ConcurrentDictionary<int, IRegisteredSchema> cacheById
            = new ConcurrentDictionary<int, IRegisteredSchema>();
        
        private readonly SemaphoreSlim cacheSemaphore = new SemaphoreSlim(1,1);

        public SchemaRegistryManager(ISchemaRegistryClient registryClient)
        {
            this.registryClient = registryClient;
        }

        public async Task<IRegisteredSchema> GetSchemaAsync(string subject, int version)
        {
            var schema = await this.registryClient
                .GetRegisteredSchemaAsync(subject, version)
                .ConfigureAwait(false);

            return new RegisteredSchemaWrapper(schema);
        }

        public async ValueTask<IRegisteredSchema> GetSchemaAsync(int id)
        {
            if (this.cacheById.TryGetValue(id, out var cached))
            {
                return cached;
            }

            await this.cacheSemaphore.WaitAsync().ConfigureAwait(false);

            try
            {
                if (this.cacheById.TryGetValue(id, out cached))
                {
                    return cached;
                }

                var versions = await this
                    .RequestListOfAsync<SubjectVersion>($"/schemas/ids/{id}/versions", HttpMethod.Get)
                    .ConfigureAwait(false);

                if (versions.Count == 0)
                {
                    throw new InvalidOperationException($"There is no Schema Registry for schema id {id}");
                }

                var schema = await this
                    .GetSchemaAsync(versions[0].Subject, versions[0].Version)
                    .ConfigureAwait(false);

                return this.cacheById.GetOrAdd(id, schema);
            }
            finally
            {
                this.cacheSemaphore.Release();
            }
        }

        //This method should be removed in future releases, when the ISchemaRegistryClient implement the /schemas/ids/{id}/versions endpoint 
        private Task<List<T>> RequestListOfAsync<T>(string url, HttpMethod method)
        {
            var restServiceInfo = this.registryClient.GetType().GetField(
                "restService",
                BindingFlags.Instance | BindingFlags.NonPublic);

            var restClient = restServiceInfo!.GetValue(this.registryClient);

            var methodInfo = restClient.GetType().GetMethod("RequestListOfAsync", BindingFlags.Instance | BindingFlags.NonPublic);

            return (Task<List<T>>) methodInfo!
                .MakeGenericMethod(typeof(T))
                .Invoke(restClient, new object[] { url, method, Array.Empty<object>() });
        }

        public void Dispose()
        {
            this.registryClient?.Dispose();
        }

        private class SubjectVersion
        {
            public string Subject { get; set; }
            public int Version { get; set; }
        }
    }
}
