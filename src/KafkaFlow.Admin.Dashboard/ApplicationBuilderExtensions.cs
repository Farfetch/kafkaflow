namespace KafkaFlow.Admin.Dashboard
{
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
            app.Map(
                pathMatch,
                builder =>
                {
                    var provider = new ManifestEmbeddedFileProvider(
                        Assembly.GetAssembly(typeof(ApplicationBuilderExtensions)),
                        "ClientApp/dist");

                    builder.UseStaticFiles(new StaticFileOptions { FileProvider = provider });

                    builder.Run(
                        async context =>
                        {
                            if (context.Request.Path == "/" || context.Request.Path == string.Empty)
                            {
                                await context.Response.SendFileAsync(provider.GetFileInfo("index.html"));
                            }
                        });
                });

            return app;
        }
    }
}
