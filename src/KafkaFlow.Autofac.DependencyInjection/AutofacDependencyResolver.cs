using System;
using Autofac;

namespace KafkaFlow.Autofac.DependencyInjection.KafkaFlow.Autofac
{
    /// <summary>
    /// Autofac implementation of <see cref="IDependencyResolver"/>
    /// </summary>
    public class AutofacDependencyResolver : IDependencyResolver
    {
        private readonly ILifetimeScope scope;

        /// <summary>
        /// Creates a <see cref="AutofacDependencyResolver"/> instance
        /// </summary>
        /// <param name="scope"></param>
        public AutofacDependencyResolver(ILifetimeScope scope) => this.scope = scope;

        /// <inheritdoc />
        public IDependencyResolverScope CreateScope()
        {
            return new AutofacDependencyResolverScope(this.scope.BeginLifetimeScope());
        }

        /// <inheritdoc />
        public object Resolve(Type type) => this.scope.Resolve(type);
    }
}