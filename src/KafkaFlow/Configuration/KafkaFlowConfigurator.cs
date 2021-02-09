namespace KafkaFlow.Configuration
{
    using System;
    using KafkaFlow.Consumers;
    using KafkaFlow.Producers;

    /// <summary>
    /// A class to configure KafkaFlow
    /// </summary>
    public class KafkaFlowConfigurator
    {
        private readonly KafkaConfiguration configuration;

        /// <summary>
        /// Creates a <see cref="KafkaFlowConfigurator"/> instance
        /// </summary>
        /// <param name="dependencyConfigurator">Dependency injection configurator</param>
        /// <param name="kafka">A handler to setup the configuration</param>
        public KafkaFlowConfigurator(
            IDependencyConfigurator dependencyConfigurator,
            Action<IKafkaConfigurationBuilder> kafka)
        {
            var builder = new KafkaConfigurationBuilder(dependencyConfigurator);

            kafka(builder);

            this.configuration = builder.Build();
        }

        /// <summary>
        /// Creates the KafkaFlow bus
        /// </summary>
        /// <param name="resolver">The <see cref="IDependencyResolver"/> to be used by the framework</param>
        /// <returns></returns>
        public IKafkaBus CreateBus(IDependencyResolver resolver)
        {
            var scope = resolver.CreateScope();

            return new KafkaBus(
                scope.Resolver,
                scope.Resolver.Resolve<IConsumerManager>(),
                scope.Resolver.Resolve<IProducerAccessor>(),
                scope.Resolver.Resolve<ILogHandler>(),
                this.configuration);
        }
    }
}
