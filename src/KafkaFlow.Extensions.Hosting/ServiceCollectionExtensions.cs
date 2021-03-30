namespace KafkaFlow
{
    using System;
    using global::Microsoft.Extensions.DependencyInjection;
    using KafkaFlow.Configuration;

    /// <summary>
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Configures KafkaFlow to run as a Hosted Service
        /// </summary>
        /// <param name="services">Instance of <see cref="IServiceCollection"/></param>
        /// <param name="kafka">A handler to configure KafkaFlow</param>
        /// <returns></returns>
        public static IServiceCollection AddKafkaFlowHostedService(
            this IServiceCollection services,
            Action<IKafkaConfigurationBuilder> kafka)
        {
            return services
                .AddHostedService<KafkaFlowHostedService>()
                .AddKafka(kafka);
        }
    }
}
