namespace KafkaFlow.Admin.Dashboard
{
    using System;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Builder;

    internal class DashboardConfiguration
    {
        private readonly List<Type> middlewares;

        public DashboardConfiguration(List<Type> middlewares, Action<IEndpointConventionBuilder> endpointHandler)
        {
            this.middlewares = middlewares;
            this.EndpointHandler = endpointHandler;
        }

        public IReadOnlyCollection<Type> Middlewares => this.middlewares.AsReadOnly();

        public Action<IEndpointConventionBuilder> EndpointHandler { get; }
    }
}
