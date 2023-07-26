namespace KafkaFlow.Admin.Dashboard
{
    using System;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;

    internal class DashboardConfigurationBuilder : IDashboardConfigurationBuilder
    {
        private readonly PathString basePath = "/kafkaflow";

        private Action<IApplicationBuilder> requestHandler = _ => { };
        private Action<IEndpointConventionBuilder> endpointHandler = _ => { };

        public IDashboardConfigurationBuilder ConfigureRequestPipeline(Action<IApplicationBuilder> requestHandler)
        {
            this.requestHandler = requestHandler;
            return this;
        }

        public IDashboardConfigurationBuilder ConfigureEndpoint(Action<IEndpointConventionBuilder> endpointHandler)
        {
            this.endpointHandler = endpointHandler;
            return this;
        }

        public DashboardConfiguration Build()
        {
            return new(this.basePath, this.requestHandler, this.endpointHandler);
        }
    }
}
