using System;

namespace KafkaFlow;

/// <summary>
/// Represents the interface of a dependency injection resolver scope
/// </summary>
public interface IDependencyResolverScope : IDisposable
{
    /// <summary>
    /// Gets the dependency injection resolver
    /// </summary>
    IDependencyResolver Resolver { get; }
}
