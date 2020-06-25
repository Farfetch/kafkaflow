namespace KafkaFlow
{
    using System;

    public interface IDependencyResolverScope : IDisposable
    {
        IDependencyResolver Resolver { get; }
    }
}
