using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace KafkaFlow.Admin.Dashboard
{
    internal class DashboardConfigurationBuilder : IDashboardConfigurationBuilder
    {
        private readonly PathString _basePath = "/kafkaflow";

        private Action<IApplicationBuilder> _requestHandler = _ => { };
        private Action<IEndpointConventionBuilder> _endpointHandler = _ => { };

        public IDashboardConfigurationBuilder ConfigureRequestPipeline(Action<IApplicationBuilder> requestHandler)
        {
            _requestHandler = requestHandler;
            return this;
        }

        public IDashboardConfigurationBuilder ConfigureEndpoint(Action<IEndpointConventionBuilder> endpointHandler)
        {
            _endpointHandler = endpointHandler;
            return this;
        }

        public DashboardConfiguration Build()
        {
            return new(_basePath, _requestHandler, _endpointHandler);
        }
    }
}
