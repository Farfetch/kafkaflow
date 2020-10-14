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
            this.middlewaresFactories.Add(factory);
            return this;
        }

        public IConsumerMiddlewareConfigurationBuilder AddAtBeginning<T>(Factory<T> factory) where T : class, IMessageMiddleware
        {
            this.middlewaresFactories.Insert(0, factory);
            return this;
        }

        public IConsumerMiddlewareConfigurationBuilder Add<T>() where T : class, IMessageMiddleware
        {
            this.RegisterType<T>();
            this.middlewaresFactories.Add(resolver => resolver.Resolve<T>());
            return this;
        }

        public IConsumerMiddlewareConfigurationBuilder AddAtBeginning<T>() where T : class, IMessageMiddleware
        {
            this.RegisterType<T>();
            this.middlewaresFactories.Insert(0, resolver => resolver.Resolve<T>());
            return this;
        }        

        public MiddlewareConfiguration Build() => new MiddlewareConfiguration(this.middlewaresFactories);

        private void RegisterType<T>() where T : class, IMessageMiddleware
        {
            this.DependencyConfigurator.AddTransient<T>();
        }
    }
}
