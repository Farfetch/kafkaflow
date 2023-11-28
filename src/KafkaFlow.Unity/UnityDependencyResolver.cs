using System;
using System.Collections.Generic;
using global::Unity;

namespace KafkaFlow.Unity
{
    /// <summary>
    /// Unity implementation of <see cref="IDependencyResolver"/>
    /// </summary>
    public class UnityDependencyResolver : IDependencyResolver
    {
        private readonly IUnityContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnityDependencyResolver"/> class.
        /// </summary>
        /// <param name="container">A Unity container instance</param>
        public UnityDependencyResolver(IUnityContainer container) => _container = container;

        /// <inheritdoc cref="IDependencyResolver.CreateScope"/>
        public IDependencyResolverScope CreateScope() =>
            new UnityDependencyResolverScope(_container.CreateChildContainer());

        /// <inheritdoc cref="IDependencyResolver.Resolve"/>
        public object Resolve(Type type) => _container.Resolve(type);

        /// <inheritdoc cref="IDependencyResolver.ResolveAll"/>
        public IEnumerable<object> ResolveAll(Type type)
        {
            yield return _container.Resolve(type);

            foreach (var instance in _container.ResolveAll(type))
            {
                yield return instance;
            }
        }
    }
}
