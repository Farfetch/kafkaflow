namespace KafkaFlow.Unity
{
    using System;
    using System.Collections.Generic;
    using global::Unity;

    /// <summary>
    /// Unity implementation of <see cref="IDependencyResolver"/>
    /// </summary>
    public class UnityDependencyResolver : IDependencyResolver
    {
        private readonly IUnityContainer container;

        /// <summary>
        /// Creates a <see cref="UnityDependencyResolver"/> instance
        /// </summary>
        /// <param name="container">A Unity container instance</param>
        public UnityDependencyResolver(IUnityContainer container) => this.container = container;

        /// <summary>
        /// Creates a container scope
        /// </summary>
        /// <returns></returns>
        public IDependencyResolverScope CreateScope() =>
            new UnityDependencyResolverScope(this.container.CreateChildContainer());

        /// <summary>
        /// Gets an instances of the passed type
        /// </summary>
        /// <param name="type">The type to be created</param>
        /// <returns></returns>
        public object Resolve(Type type) => this.container.Resolve(type);
    }
}
