namespace KafkaFlow.Configuration
{
    using System;

    internal class MiddlewareInstanceContainer<T, TConfiguration> : IMiddlewareInstanceContainer<TConfiguration>
    {
        private readonly object sync = new();
        private readonly Func<IDependencyResolver, TConfiguration, IMessageMiddleware> factory;

        private IMessageMiddleware instance;

        public MiddlewareInstanceContainer(Guid id, Func<IDependencyResolver, TConfiguration, IMessageMiddleware> factory)
        {
            this.Id = id;
            this.factory = factory;
        }

        public Guid Id { get; }

        public IMessageMiddleware GetInstance(IDependencyResolver resolver, TConfiguration configuration)
        {
            if (this.instance is not null)
            {
                return this.instance;
            }

            lock (this.sync)
            {
                if (this.instance is not null)
                {
                    return this.instance;
                }

                this.instance = this.factory(resolver, configuration);
            }

            return this.instance;
        }
    }
}
