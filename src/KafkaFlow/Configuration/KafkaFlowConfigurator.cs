namespace KafkaFlow.Configuration
{
    using System;
    using KafkaFlow.Consumers;

    /// <summary>
    /// Class configurator for kafkaflow clients
    /// </summary>
    public class KafkaFlowConfigurator
    {
        /// <summary>
        /// KafkaFlowConfigurator constructor
        /// </summary>
        /// <param name="dependencyConfigurator">Dependency injection configurator</param>
        /// <param name="kafka">Action to be done over instance of IKafkaConfigurationBuilder</param>
        public KafkaFlowConfigurator(
            IDependencyConfigurator dependencyConfigurator,
            Action<IKafkaConfigurationBuilder> kafka)
        {
            var builder = new KafkaConfigurationBuilder(dependencyConfigurator);

            kafka(builder);

            dependencyConfigurator.AddSingleton(builder.Build());
        }

        /// <summary>
        /// Creates kafka bus using the instance of <see cref="IDependencyResolver"/> informed 
        /// </summary>
        /// <param name="resolver">Implementation of <see cref="IDependencyResolver"/></param>
        /// <returns></returns>
        public IKafkaBus CreateBus(IDependencyResolver resolver)
        {
            var scope = resolver.CreateScope();

            return new KafkaBus(
                scope.Resolver,
                scope.Resolver.Resolve<IConsumerManager>(),
                scope.Resolver.Resolve<ILogHandler>(),
                scope.Resolver.Resolve<KafkaConfiguration>());
        }
    }
}
