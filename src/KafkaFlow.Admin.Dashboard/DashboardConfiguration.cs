namespace KafkaFlow.Admin.Dashboard
{
    using System;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;

    internal class DashboardConfiguration
    {
        public DashboardConfiguration(
            PathString basePath,
            Action<IApplicationBuilder> requestHandler,
            Action<IEndpointConventionBuilder> endpointHandler)
        {
            this.BasePath = basePath;
            this.RequestHandler = requestHandler;
            this.EndpointHandler = endpointHandler;
        }

        public PathString BasePath { get; }

        public Action<IApplicationBuilder> RequestHandler { get; }

        public Action<IEndpointConventionBuilder> EndpointHandler { get; }
    }
}
