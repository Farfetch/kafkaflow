namespace KafkaFlow.Configuration
{
    using System.Collections.Generic;

    internal class ConsumerMiddlewareConfigurationBuilder
        : IConsumerMiddlewareConfigurationBuilder
    {
        public IDependencyConfigurator DependencyConfigurator { get; }

        private readonly List<Factory<IMessageMiddleware>> middlewaresFactories =
            new List<Factory<IMessageMiddleware>>();

        public ConsumerMiddlewareConfigurationBuilder(IDependencyConfigurator dependencyConfigurator)
        {
            this.DependencyConfigurator = dependencyConfigurator;
        }

        public IConsumerMiddlewareConfigurationBuilder Add<T>(Factory<T> factory) where T : class, IMessageMiddleware
        {
            this.DependencyConfigurator.AddTransient<T>();
            this.middlewaresFactories.Add(factory);
            return this;
        }

        public IConsumerMiddlewareConfigurationBuilder Add<T>() where T : class, IMessageMiddleware
        {
            return this.Add(resolver => resolver.Resolve<T>());
        }

        public MiddlewareConfiguration Build() => new MiddlewareConfiguration(this.middlewaresFactories);
    }
}
