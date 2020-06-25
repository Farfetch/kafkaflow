namespace KafkaFlow
{
    using System;

    public interface IDependencyConfigurator
    {
        IDependencyConfigurator Add(Type serviceType, Type implementationType, InstanceLifetime lifetime);
        
        IDependencyConfigurator Add<TService, TImplementation>(InstanceLifetime lifetime)
            where TService : class
            where TImplementation : class, TService;

        IDependencyConfigurator Add<TService>(InstanceLifetime lifetime)
            where TService : class;

        IDependencyConfigurator Add<TImplementation>(TImplementation service)
            where TImplementation : class;

        IDependencyConfigurator Add<TImplementation>(
            Type serviceType,
            Func<IDependencyResolver, TImplementation> factory,
            InstanceLifetime lifetime);
    }
}
