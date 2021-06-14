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
        /// Enable the KafkaFlow dashboard
        /// </summary>
        /// <param name="app">Instance of <see cref="IApplicationBuilder"/></param>
        /// <returns></returns>
        public static IApplicationBuilder UseKafkaFlowDashboard(this IApplicationBuilder app)
        {
            app.Map(
                "/kafka-flow",
                builder =>
                {
                    var provider = new ManifestEmbeddedFileProvider(
                        Assembly.GetAssembly(typeof(ApplicationBuilderExtensions)),
                        "ClientApp/dist");

                    builder.UseStaticFiles(new StaticFileOptions { FileProvider = provider });

                    builder.Run(
                        async context =>
                        {
                            if (context.Request.Path == "/")
                            {
                                await context.Response.SendFileAsync(provider.GetFileInfo("index.html"));
                            }
                        });
                });

            return app;
        }
    }
}
