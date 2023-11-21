using System;

namespace KafkaFlow.Configuration
{
    internal interface IMiddlewareInstanceContainer
    {
        Guid Id { get; }

        IMessageMiddleware GetInstance(IDependencyResolver resolver);
    }
}
