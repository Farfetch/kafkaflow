namespace KafkaFlow.Microsoft.DependencyInjection
{
    using System;
    using System.Collections.Generic;
    using global::Microsoft.Extensions.DependencyInjection;

    internal class MicrosoftDependencyResolver : IDependencyResolver
    {
        private readonly IServiceProvider serviceProvider;

        public MicrosoftDependencyResolver(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public object Resolve(Type type)
        {
            return this.serviceProvider.GetService(type);
        }

        public IDependencyResolverScope CreateScope()
        {
            return new MicrosoftDependencyResolverScope(this.serviceProvider.CreateScope());
        }
    }
}
