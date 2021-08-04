namespace KafkaFlow.Admin.Dashboard
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.AspNetCore.Builder;

    internal class DashboardConfigurationBuilder : IDashboardConfigurationBuilder
    {
        private readonly List<Type> middlewares = new();
        private Action<IEndpointConventionBuilder> routeMappedHandler = _ => { };

        public IDashboardConfigurationBuilder AddMiddleware(Type middleware)
        {
            this.middlewares.Add(middleware);
            return this;
        }

        public IDashboardConfigurationBuilder AddMiddlewares(IEnumerable<Type> typeMiddlewares)
        {
            this.middlewares.AddRange(typeMiddlewares);
            return this;
        }

        public IDashboardConfigurationBuilder WithPostMapHandler(Action<IEndpointConventionBuilder> handler)
        {
            this.routeMappedHandler = handler;
            return this;
        }

        public DashboardConfiguration Build()
        {
            return new(this.middlewares, this.routeMappedHandler);
        }
    }
}
