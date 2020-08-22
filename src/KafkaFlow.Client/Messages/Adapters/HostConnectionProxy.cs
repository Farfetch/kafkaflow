namespace KafkaFlow.Client.Messages.Adapters
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
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
            var interfaceType = typeof(IHostConnectionAdapter<,>).MakeGenericType(request.GetType(), typeof(TResponse));

            var adapter = this.GetAdapterInstance<TResponse>(request, interfaceType);

            return adapter.SendAsync(request);
        }

        private AdapterExecutor<TResponse> GetAdapterInstance<TResponse>(IClientRequest request, Type interfaceType)
            where TResponse : IClientResponse
        {
            return (AdapterExecutor<TResponse>) this.adaptersCache.GetOrAdd(
                interfaceType,
                _ =>
                {
                    var versionRange = this.hostCapabilities.GetVersionRange(request.ApiKey);

                    var adapterType = GetAdapterType(interfaceType, versionRange);

                    if (adapterType is null)
                    {
                        throw new NotSupportedException($"Your Kafka host does not support {request.GetType().Name} API");
                    }

                    var adapter = Activator.CreateInstance(adapterType, this.connection);

                    var executorType = typeof(AdapterExecutor<,>).MakeGenericType(request.GetType(), typeof(TResponse));
                    var executor = (AdapterExecutor<TResponse>) Activator.CreateInstance(executorType, adapter);

                    return executor;
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
                        type.GetCustomAttribute<ApiVersionAttribute>().Version <= versionRange.Max)
                .OrderByDescending(type => type.GetCustomAttribute<ApiVersionAttribute>().Version)
                .FirstOrDefault();
        }

        public void Dispose()
        {
            this.connection.Dispose();
        }
    }

    internal abstract class AdapterExecutor<TResponse>
        where TResponse : IClientResponse
    {
        public abstract Task<TResponse> SendAsync(IClientRequest<TResponse> request);
    }

    internal class AdapterExecutor<TRequest, TResponse> : AdapterExecutor<TResponse>
        where TRequest : IClientRequest<TResponse>
        where TResponse : IClientResponse
    {
        private readonly object adapter;

        public AdapterExecutor(object adapter)
        {
            this.adapter = adapter;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Task<TResponse> SendAsync(IClientRequest<TResponse> request)
        {
            return ((IHostConnectionAdapter<TRequest, TResponse>) this.adapter).SendAsync((TRequest) request);
        }
    }
}
