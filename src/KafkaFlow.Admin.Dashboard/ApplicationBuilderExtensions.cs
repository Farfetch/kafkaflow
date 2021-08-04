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
            return app.UseKafkaFlowDashboard("/kafka-flow");
        }

        /// <summary>
        /// Enable the KafkaFlow dashboard
        /// </summary>
        /// <param name="app">Instance of <see cref="IApplicationBuilder"/></param>
        /// <param name="pathMatch">The request path to match.</param>
        /// <returns></returns>
        public static IApplicationBuilder UseKafkaFlowDashboard(this IApplicationBuilder app, PathString pathMatch)
        {
            return app.UseKafkaFlowDashboard(pathMatch, null);
        }

        /// <summary>
        /// Enable the KafkaFlow dashboard
        /// </summary>
        /// <param name="app">Instance of <see cref="IApplicationBuilder"/></param>
        /// <param name="pathMatch">The request path to match.</param>
        /// <param name="actionBuilder">The handler to be executed after the dashboard url is mapped.</param>
        /// <returns></returns>
        public static IApplicationBuilder UseKafkaFlowDashboard(
            this IApplicationBuilder app,
            PathString pathMatch,
            Action<IDashboardConfigurationBuilder> actionBuilder)
        {
            var builder = new DashboardConfigurationBuilder();
            actionBuilder?.Invoke(builder);
            var configuration = builder.Build();

            app.Map(
                pathMatch,
                appBuilder =>
                {
                    var provider = new ManifestEmbeddedFileProvider(
                        Assembly.GetAssembly(typeof(KafkaFlow.Admin.Dashboard.ApplicationBuilderExtensions)),
                        "ClientApp/dist");

                    appBuilder
                        .UseStaticFiles(new StaticFileOptions { FileProvider = provider })
                        .UseRouting();

                    foreach (var middleware in configuration.Middlewares)
                    {
                        appBuilder.UseMiddleware(middleware);
                    }

                    appBuilder.UseEndpoints(routeBuilder =>
                    {
                        var endpoint = routeBuilder.Map(
                            "/",
                            async context => await context.Response.SendFileAsync(provider.GetFileInfo("index.html")));

                        configuration.RouteMappedHandler?.Invoke(endpoint);
                    });
                });
            return app;
        }
    }
}
