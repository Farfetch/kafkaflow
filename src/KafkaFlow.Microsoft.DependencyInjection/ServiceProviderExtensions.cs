using System;
using global::Microsoft.Extensions.DependencyInjection;
using KafkaFlow.Configuration;

namespace KafkaFlow;

/// <summary>
/// Extension methods over IServiceProvider
/// </summary>
public static class ServiceProviderExtensions
{
    /// <summary>
    /// Creates a KafkaFlow bus
    /// </summary>
    /// <param name="provider">Instance of <see cref="IServiceProvider"/></param>
    /// <returns><see cref="IKafkaBus"/>A KafkaFlow bus</returns>
    public static IKafkaBus CreateKafkaBus(this IServiceProvider provider)
    {
        var resolver = provider.GetRequiredService<IDependencyResolver>();
        return provider.GetRequiredService<KafkaFlowConfigurator>().CreateBus(resolver);
    }
}
