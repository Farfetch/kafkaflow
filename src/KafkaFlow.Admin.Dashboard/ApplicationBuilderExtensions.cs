namespace KafkaFlow.Admin.Dashboard
{
    using System;
    using System.Reflection;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.FileProviders;

    /// <summary>
    /// Extension methods over IApplicationBuilder
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Enable the KafkaFlow dashboard. The path will be `/kafka-flow`
        /// </summary>
        /// <param name="app">Instance of <see cref="IApplicationBuilder"/></param>
        /// <returns></returns>
        public static IApplicationBuilder UseKafkaFlowDashboard(this IApplicationBuilder app)
        {
            return app.UseKafkaFlowDashboard((Action<IDashboardConfigurationBuilder>) null);
        }

        /// <summary>
        /// Enable the KafkaFlow dashboard
        /// </summary>
        /// <param name="app">Instance of <see cref="IApplicationBuilder"/></param>
        /// <param name="pathMatch">Do nothing.</param>
        /// <returns></returns>
        [Obsolete("This method was deprecated and the pathMatch parameter will not change the dashboard base path.", true)]
        public static IApplicationBuilder UseKafkaFlowDashboard(this IApplicationBuilder app, PathString pathMatch)
        {
            return app.UseKafkaFlowDashboard((Action<IDashboardConfigurationBuilder>) null);
        }

        /// <summary>
        /// Enable the KafkaFlow dashboard
        /// </summary>
        /// <param name="app">Instance of <see cref="IApplicationBuilder"/></param>
        /// <param name="dashboard">A handler to configure the dashboard.</param>
        /// <returns></returns>
        public static IApplicationBuilder UseKafkaFlowDashboard(
            this IApplicationBuilder app,
            Action<IDashboardConfigurationBuilder> dashboard)
        {
            var builder = new DashboardConfigurationBuilder();
            dashboard?.Invoke(builder);
            var configuration = builder.Build();

            app.Map(
                configuration.BasePath,
                appBuilder =>
                {
                    var provider = new ManifestEmbeddedFileProvider(
                        Assembly.GetAssembly(typeof(KafkaFlow.Admin.Dashboard.ApplicationBuilderExtensions)),
                        "ClientApp/dist");

                    appBuilder
                        .UseStaticFiles(new StaticFileOptions { FileProvider = provider })
                        .UseRouting();

                    configuration.RequestHandler?.Invoke(appBuilder);

                    appBuilder.UseEndpoints(routeBuilder =>
                    {
                        var endpoint = routeBuilder.Map(
                            "/",
                            async context => await context.Response.SendFileAsync(provider.GetFileInfo("index.html")));

                        configuration.EndpointHandler?.Invoke(endpoint);
                    });
                });
            return app;
        }
    }
}
