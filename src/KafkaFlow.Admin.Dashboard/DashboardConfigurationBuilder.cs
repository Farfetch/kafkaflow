namespace KafkaFlow.Admin.Dashboard
{
    using System;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Builder;

    internal class DashboardConfigurationBuilder : IDashboardConfigurationBuilder
    {
        private readonly List<Type> middlewares = new();
        private Action<IEndpointConventionBuilder> endpointHandler = _ => { };

        public IDashboardConfigurationBuilder UseMiddleware<T>()
        {
            this.middlewares.Add(typeof(T));
            return this;
        }

        public IDashboardConfigurationBuilder ConfigureEndpoint(Action<IEndpointConventionBuilder> endpointHandler)
        {
            this.endpointHandler = endpointHandler;
            return this;
        }

        public DashboardConfiguration Build()
        {
            return new(this.middlewares, this.endpointHandler);
        }
    }
}
