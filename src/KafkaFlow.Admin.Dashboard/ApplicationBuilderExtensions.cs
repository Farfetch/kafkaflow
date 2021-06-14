namespace KafkaFlow.Admin.Dashboard
{
    using Microsoft.AspNetCore.Builder;

    /// <summary>
    /// Extension methods over IApplicationBuilder
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Enable the KafkaFlow dashboard
        /// </summary>
        /// <param name="app">Instance of <see cref="IApplicationBuilder"/></param>
        /// <returns></returns>
        public static IApplicationBuilder UseKafkaFlowDashboard(this IApplicationBuilder app)
        {
            app
                .UseMiddleware<ServeClientFilesMiddleware>();

            return app;
        }
    }
}
