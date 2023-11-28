using System;
using Microsoft.AspNetCore.Builder;

namespace KafkaFlow.Admin.Dashboard
{
    /// <summary>
    /// Used to build the dashboard configuration
    /// </summary>
    public interface IDashboardConfigurationBuilder
    {
        /// <summary>
        /// Use this method to configure the dashboard request pipeline
        /// </summary>
        /// <param name="requestHandler">A handler to configure the request pipeline</param>
        /// <returns>The <see cref="IDashboardConfigurationBuilder"/> instance.</returns>
        IDashboardConfigurationBuilder ConfigureRequestPipeline(Action<IApplicationBuilder> requestHandler);

        /// <summary>
        /// Use this method to configure the dashboard endpoint
        /// </summary>
        /// <param name="endpointHandler">A handler to configure the endpoint</param>
        /// <returns>The <see cref="IEndpointConventionBuilder"/> instance.</returns>
        IDashboardConfigurationBuilder ConfigureEndpoint(Action<IEndpointConventionBuilder> endpointHandler);
    }
}
