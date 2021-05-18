namespace KafkaFlow.Admin.Dashboard
{
    using System.Text.Json.Serialization;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Extension methods over IServiceCollection
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Configures a KafkaFlow Dashboard
        /// </summary>
        /// <param name="services">Instance of <see cref="IServiceCollection"/></param>
        /// <returns></returns>
        public static IServiceCollection AddKafkaFlowDashboard(this IServiceCollection services)
        {
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "AngularFiles/dist";
            });

            services
                .AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    options.JsonSerializerOptions.IgnoreNullValues = true;
                });

            return services;
        }
    }
}
