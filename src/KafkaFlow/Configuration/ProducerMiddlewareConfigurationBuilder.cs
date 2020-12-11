namespace KafkaFlow.Configuration
{
    using System.Collections.Generic;

    internal class ProducerMiddlewareConfigurationBuilder
        : IProducerMiddlewareConfigurationBuilder
    {
        public IDependencyConfigurator DependencyConfigurator { get; }

        private readonly List<Factory<IMessageMiddleware>> middlewaresFactories =
            new List<Factory<IMessageMiddleware>>();

        private bool cloneContext = true;

        public ProducerMiddlewareConfigurationBuilder(IDependencyConfigurator dependencyConfigurator)
        {
            this.DependencyConfigurator = dependencyConfigurator;
        }

        public IProducerMiddlewareConfigurationBuilder Add<T>(Factory<T> factory) where T : class, IMessageMiddleware
        {
            this.middlewaresFactories.Add(factory);
            return this;
        }

        public IProducerMiddlewareConfigurationBuilder Add<T>() where T : class, IMessageMiddleware
        {
            this.DependencyConfigurator.AddTransient<T>();
            this.middlewaresFactories.Add(resolver => resolver.Resolve<T>());
            return this;
        }

        public IProducerMiddlewareConfigurationBuilder WithCloneContext(bool cloneContext)
        {
            this.cloneContext = cloneContext;
            return this;
        }

        public MiddlewareConfiguration Build() =>
            new MiddlewareConfiguration(this.middlewaresFactories, this.cloneContext);
    }
}
