namespace KafkaFlow.Client.Messages.Adapters
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using KafkaFlow.Client.Messages;
    using KafkaFlow.Client.Protocol;

    internal class HostConnectionProxy : IHostConnectionProxy
    {
        private readonly IKafkaHostConnection connection;
        private readonly IHostCapabilities hostCapabilities;

        private readonly ConcurrentDictionary<Type, object> adaptersCache = new ConcurrentDictionary<Type, object>();

        public HostConnectionProxy(IKafkaHostConnection connection, IHostCapabilities hostCapabilities)
        {
            this.connection = connection;
            this.hostCapabilities = hostCapabilities;
        }

        public Task<TResponse> SendAsync<TResponse>(IClientRequest<TResponse> request)
            where TResponse : IClientResponse
        {
            var interfaceType = typeof(IHostConnectionAdapter<IClientRequest<TResponse>, TResponse>);

            var adapter = (IHostConnectionAdapter<IClientRequest<TResponse>, TResponse>) this.GetAdapterInstance(request, interfaceType);

            return adapter.SendAsync(request);
        }

        private object GetAdapterInstance(IClientRequest request, Type interfaceType)
        {
            return this.adaptersCache.GetOrAdd(
                interfaceType,
                _ =>
                {
                    var versionRange = this.hostCapabilities.GetVersionRange(request.ApiKey);

                    var adapterType = GetAdapterType(interfaceType, versionRange);

                    if (adapterType is null)
                    {
                        throw new NotSupportedException($"Your Kafka host does not support {request.GetType().Name} API");
                    }

                    return Activator.CreateInstance(adapterType, this.connection);
                });
        }

        private static Type GetAdapterType(Type interfaceType, ApiVersionRange versionRange)
        {
            return interfaceType.Assembly
                .GetTypes()
                .Where(
                    type =>
                        type.IsClass &&
                        interfaceType.IsAssignableFrom(type) &&
                        type.GetCustomAttribute<ApiVersionAttribute>().Version >= versionRange.Min &&
                        type.GetCustomAttribute<ApiVersionAttribute>().Version <= versionRange.Min)
                .OrderByDescending(type => type.GetCustomAttribute<ApiVersionAttribute>().Version)
                .FirstOrDefault();
        }

        public void Dispose()
        {
            this.connection.Dispose();
        }
    }
}
