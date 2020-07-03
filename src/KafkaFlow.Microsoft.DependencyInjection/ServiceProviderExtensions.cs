namespace KafkaFlow
{
    using System;
    using global::Microsoft.Extensions.DependencyInjection;
    using KafkaFlow.Configuration;

    /// <summary>
    /// Extension methods over IServiceProvider
    /// </summary>
    public static class ServiceProviderExtensions
    {
        /// <summary>
        /// Creates a kafka bus using a instance of KafkaFlowConfigurator
        /// </summary>
        /// <param name="provider">Instance of <see cref="IServiceProvider"/></param>
        /// <returns><see cref="IKafkaBus"/> created</returns>
        public static IKafkaBus CreateKafkaBus(this IServiceProvider provider)
        {
            var resolver = provider.GetRequiredService<IDependencyResolver>();
            return provider.GetRequiredService<KafkaFlowConfigurator>().CreateBus(resolver);
        }
    }
}
