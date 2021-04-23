namespace KafkaFlow
{
    using System;
    using Microsoft.Extensions.DependencyInjection;

    internal class MicrosoftDependencyConfigurator : IDependencyConfigurator
    {
        private readonly IServiceCollection services;

        public MicrosoftDependencyConfigurator(IServiceCollection services)
        {
            this.services = services;
            this.services.AddSingleton<IDependencyResolver>(provider => new MicrosoftDependencyResolver(provider));
        }

        public IDependencyConfigurator Add(
            Type serviceType,
            Type implementationType,
            InstanceLifetime lifetime)
        {
            this.services.Add(
                ServiceDescriptor.Describe(
                    serviceType,
                    implementationType,
                    ParseLifetime(lifetime)));

            return this;
        }

        public IDependencyConfigurator Add<TService, TImplementation>(InstanceLifetime lifetime)
            where TService : class
            where TImplementation : class, TService
        {
            this.services.Add(
                ServiceDescriptor.Describe(
                    typeof(TService),
                    typeof(TImplementation),
                    ParseLifetime(lifetime)));

            return this;
        }

        public IDependencyConfigurator Add<TService>(InstanceLifetime lifetime)
            where TService : class
        {
            this.services.Add(
                ServiceDescriptor.Describe(
                    typeof(TService),
                    typeof(TService),
                    ParseLifetime(lifetime)));

            return this;
        }

        public IDependencyConfigurator Add<TImplementation>(TImplementation service)
            where TImplementation : class
        {
            this.services.AddSingleton(service);
            return this;
        }

        public IDependencyConfigurator Add<TImplementation>(
            Type serviceType,
            Func<IDependencyResolver, TImplementation> factory,
            InstanceLifetime lifetime)
        {
            this.services.Add(
                ServiceDescriptor.Describe(
                    serviceType,
                    provider => factory(new MicrosoftDependencyResolver(provider)),
                    ParseLifetime(lifetime)));

            return this;
        }

        private static ServiceLifetime ParseLifetime(InstanceLifetime lifetime)
        {
            switch (lifetime)
            {
                case InstanceLifetime.Singleton:
                    return ServiceLifetime.Singleton;

                case InstanceLifetime.Scoped:
                    return ServiceLifetime.Scoped;

                case InstanceLifetime.Transient:
                    return ServiceLifetime.Transient;

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(lifetime),
                        lifetime,
                        null);
            }
        }
    }
}
