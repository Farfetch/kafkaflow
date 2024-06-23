using System;
using global::Microsoft.Extensions.DependencyInjection;
using KafkaFlow.Configuration;

namespace KafkaFlow;

/// <summary>
/// Extension methods over IServiceCollection
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Configures KafkaFlow
    /// </summary>
    /// <param name="services">Instance of <see cref="IServiceCollection"/></param>
    /// <param name="kafka">A handler to configure KafkaFlow</param>
    /// <returns></returns>
    public static IServiceCollection AddKafka(
        this IServiceCollection services,
        Action<IKafkaConfigurationBuilder> kafka)
    {
        var configurator = new KafkaFlowConfigurator(
            new MicrosoftDependencyConfigurator(services),
            kafka);

        return services.AddSingleton(configurator);
    }

    /// <summary>
    /// Configures KafkaFlow
    /// </summary>
    /// <param name="services">Instance of <see cref="IServiceCollection"/></param>
    /// <param name="configure">A handler to configure KafkaFlow</param>
    /// <returns></returns>
    public static IServiceCollection AddKafka(
        this IServiceCollection services,
        Action<IServiceProvider, IKafkaConfigurationBuilder> configure)
    {
        return services.AddSingleton(sp =>
        {
            var configurator = new KafkaFlowConfigurator(
                new MicrosoftDependencyConfigurator(services),
                (builder) => configure(sp, builder));
            return configurator;
        });
    }
}
