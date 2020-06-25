namespace KafkaFlow
{
    using System;
    using global::Microsoft.Extensions.DependencyInjection;
    using KafkaFlow.Configuration;
    using KafkaFlow.Microsoft.DependencyInjection;

    /// <summary>
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddKafka(
            this IServiceCollection services,
            Action<IKafkaConfigurationBuilder> kafka)
        {
            var configurator = new KafkaFlowConfigurator(
                new MicrosoftDependencyConfigurator(services),
                kafka);

            return services.AddSingleton(configurator);
        }
    }
}
