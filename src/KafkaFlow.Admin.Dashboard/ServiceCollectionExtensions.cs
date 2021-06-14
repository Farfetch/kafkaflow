namespace KafkaFlow.Admin.Dashboard
{
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
            return services
                .AddSingleton<ServeClientFilesMiddleware>();
        }
    }
}
