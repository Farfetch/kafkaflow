using System;

namespace KafkaFlow.Configuration
{
    internal class MiddlewareInstanceContainer<T> : IMiddlewareInstanceContainer
    {
        private readonly object _sync = new();
        private readonly Factory<IMessageMiddleware> _factory;

        private IMessageMiddleware _instance;

        public MiddlewareInstanceContainer(Guid id, Factory<IMessageMiddleware> factory)
        {
            this.Id = id;
            _factory = factory;
        }

        public Guid Id { get; }

        public IMessageMiddleware GetInstance(IDependencyResolver resolver)
        {
            if (_instance is not null)
            {
                return _instance;
            }

            lock (_sync)
            {
                if (_instance is not null)
                {
                    return _instance;
                }

                _instance = _factory(resolver);
            }

            return _instance;
        }
    }
}
