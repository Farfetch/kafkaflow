namespace KafkaFlow.Admin.Dashboard
{
    using System;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Builder;

    public interface IDashboardConfigurationBuilder
    {
        IDashboardConfigurationBuilder AddMiddleware(Type middleware);

        IDashboardConfigurationBuilder AddMiddlewares(IEnumerable<Type> middlewares);

        IDashboardConfigurationBuilder WithPostMapHandler(Action<IEndpointConventionBuilder> handler);
    }
}
