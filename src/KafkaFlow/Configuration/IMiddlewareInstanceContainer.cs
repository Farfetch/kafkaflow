namespace KafkaFlow.Configuration
{
    using System;

    internal interface IMiddlewareInstanceContainer<in TConfiguration>
    {
        Guid Id { get; }

        IMessageMiddleware GetInstance(IDependencyResolver resolver, TConfiguration configuration);
    }
}
