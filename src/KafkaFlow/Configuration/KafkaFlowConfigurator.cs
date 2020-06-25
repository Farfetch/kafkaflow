namespace KafkaFlow.Configuration
{
    using System;
    using KafkaFlow.Consumers;

    public class KafkaFlowConfigurator
    {
        public KafkaFlowConfigurator(
            IDependencyConfigurator dependencyConfigurator,
            Action<IKafkaConfigurationBuilder> kafka)
        {
            var builder = new KafkaConfigurationBuilder(dependencyConfigurator);

            kafka(builder);

            dependencyConfigurator.AddSingleton(builder.Build());
        }

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
