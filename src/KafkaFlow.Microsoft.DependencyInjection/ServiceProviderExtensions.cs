namespace KafkaFlow
{
    using System;
    using global::Microsoft.Extensions.DependencyInjection;
    using KafkaFlow.Configuration;

    /// <summary>
    /// </summary>
    public static class ServiceProviderExtensions
    {
        public static IKafkaBus CreateKafkaBus(this IServiceProvider provider)
        {
            var resolver = provider.GetRequiredService<IDependencyResolver>();
            return provider.GetRequiredService<KafkaFlowConfigurator>().CreateBus(resolver);
        }
    }
}
