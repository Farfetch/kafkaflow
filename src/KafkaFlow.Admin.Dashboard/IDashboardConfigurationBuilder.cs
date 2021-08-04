namespace KafkaFlow.Admin.Dashboard
{
    using System;
    using Microsoft.AspNetCore.Builder;

    /// <summary>
    /// Used to build the dashboard configuration
    /// </summary>
    public interface IDashboardConfigurationBuilder
    {
        /// <summary>
        /// Adds a middleware type to the dashboard's request pipeline.
        /// </summary>
        /// <returns>The <see cref="IDashboardConfigurationBuilder"/> instance.</returns>
        IDashboardConfigurationBuilder UseMiddleware<T>();

        /// <summary>
        /// Use this method to configure the dashboard endpoint pipeline.
        /// </summary>
        /// <param name="endpointHandler">A handler to configure the endpoint</param>
        /// <returns>The <see cref="IDashboardConfigurationBuilder"/> instance.</returns>
        IDashboardConfigurationBuilder ConfigureEndpoint(Action<IEndpointConventionBuilder> endpointHandler);
    }
}
