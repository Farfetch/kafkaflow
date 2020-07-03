namespace KafkaFlow
{
    using System;
    using global::Microsoft.Extensions.DependencyInjection;
    using KafkaFlow.Configuration;
    using KafkaFlow.Microsoft.DependencyInjection;

    /// <summary>
    /// Extension methods over IServiceCollection
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds a singleton instance of <see cref="KafkaFlowConfigurator"/> with <see cref="MicrosoftDependencyConfigurator"/>
        /// </summary>
        /// <param name="services">Instance of <see cref="IServiceCollection"/></param>
        /// <param name="kafka">Action to be done over instance of <see cref="IKafkaConfigurationBuilder"/></param>
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
    }
}
