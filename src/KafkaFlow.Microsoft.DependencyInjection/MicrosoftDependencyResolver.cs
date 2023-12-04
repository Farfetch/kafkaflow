using System;
using System.Collections.Generic;
using global::Microsoft.Extensions.DependencyInjection;

namespace KafkaFlow;

internal class MicrosoftDependencyResolver : IDependencyResolver
{
    private readonly IServiceProvider _serviceProvider;

    public MicrosoftDependencyResolver(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public object Resolve(Type type)
    {
        return _serviceProvider.GetService(type);
    }

    public IEnumerable<object> ResolveAll(Type type)
    {
        return _serviceProvider.GetServices(type);
    }

    public IDependencyResolverScope CreateScope()
    {
        return new MicrosoftDependencyResolverScope(_serviceProvider.CreateScope());
    }
}
