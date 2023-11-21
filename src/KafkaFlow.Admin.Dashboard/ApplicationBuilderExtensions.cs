using System;
using System.Globalization;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace KafkaFlow.Admin.Dashboard
{
    /// <summary>
    /// Extension methods over IApplicationBuilder
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Enable the KafkaFlow dashboard. The path will be `/kafkaflow`
        /// </summary>
        /// <param name="app">Instance of <see cref="IApplicationBuilder"/></param>
        /// <returns></returns>
        public static IApplicationBuilder UseKafkaFlowDashboard(this IApplicationBuilder app)
        {
            return app.UseKafkaFlowDashboard(null);
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
                        Assembly.GetAssembly(typeof(ApplicationBuilderExtensions)),
                        "ClientApp/dist");

                    appBuilder
                        .UseStaticFiles(new StaticFileOptions { FileProvider = provider })
                        .UseRouting();

                    configuration.RequestHandler?.Invoke(appBuilder);

                    appBuilder.UseEndpoints(
                        routeBuilder =>
                        {
                            var jsonSettings = new JsonSerializerSettings
                            {
                                Formatting = Formatting.None,
                                ContractResolver = new DefaultContractResolver
                                {
                                    NamingStrategy = new CamelCaseNamingStrategy(),
                                },
                            };

                            var endpoint = routeBuilder.Map(
                                "/",
                                async context => await context.Response.SendFileAsync(provider.GetFileInfo("index.html")));

                            routeBuilder.MapPut(
                                "/consumers/{name}/topics/{topicName}/pause",
                                async context =>
                                {
                                    var consumerAdmin = context.RequestServices.GetRequiredService<IConsumerAdmin>();
                                    var consumerName = context.Request.RouteValues["name"].ToString();
                                    var topicName = context.Request.RouteValues["topicName"].ToString();

                                    await consumerAdmin.PauseConsumerAsync(consumerName, new[] { topicName });

                                    await context.Response.WriteAsync(string.Empty);
                                    await context.Response.CompleteAsync();
                                });

                            routeBuilder.MapPut(
                                "/consumers/{name}/topics/{topicName}/resume",
                                async context =>
                                {
                                    var consumerAdmin = context.RequestServices.GetRequiredService<IConsumerAdmin>();

                                    var consumerName = context.Request.RouteValues["name"].ToString();
                                    var topicName = context.Request.RouteValues["topicName"].ToString();

                                    await consumerAdmin.ResumeConsumerAsync(consumerName, new[] { topicName });

                                    await context.Response.WriteAsync(string.Empty);
                                    await context.Response.CompleteAsync();
                                });

                            routeBuilder.MapPut(
                                "/consumers/{name}/topics/{topicName}/reset",
                                async context =>
                                {
                                    var consumerAdmin = context.RequestServices.GetRequiredService<IConsumerAdmin>();

                                    var consumerName = context.Request.RouteValues["name"].ToString();
                                    var topicName = context.Request.RouteValues["topicName"].ToString();

                                    await consumerAdmin.ResetOffsetsAsync(consumerName, new[] { topicName });

                                    await context.Response.WriteAsync(string.Empty);
                                    await context.Response.CompleteAsync();
                                });

                            routeBuilder.MapPut(
                                "/consumers/{name}/topics/{topicName}/rewind/{date}",
                                async context =>
                                {
                                    var consumerAdmin = context.RequestServices.GetRequiredService<IConsumerAdmin>();

                                    var consumerName = context.Request.RouteValues["name"].ToString();
                                    var topicName = context.Request.RouteValues["topicName"].ToString();
                                    var date = DateTime.ParseExact(
                                        context.Request.RouteValues["date"].ToString() ?? string.Empty,
                                        "yyyy-MM-dd HH:mm:ss",
                                        CultureInfo.InvariantCulture);

                                    await consumerAdmin.RewindOffsetsAsync(consumerName, date, new[] { topicName });

                                    await context.Response.WriteAsync(string.Empty);
                                    await context.Response.CompleteAsync();
                                });

                            routeBuilder.MapPut(
                                "/consumers/{name}/stop",
                                async context =>
                                {
                                    var consumerAdmin = context.RequestServices.GetRequiredService<IConsumerAdmin>();

                                    var consumerName = context.Request.RouteValues["name"].ToString();

                                    await consumerAdmin.StopConsumerAsync(consumerName);

                                    await context.Response.WriteAsync(string.Empty);
                                    await context.Response.CompleteAsync();
                                });

                            routeBuilder.MapPut(
                                "/consumers/{name}/start",
                                async context =>
                                {
                                    var consumerAdmin = context.RequestServices.GetRequiredService<IConsumerAdmin>();

                                    var consumerName = context.Request.RouteValues["name"].ToString();

                                    await consumerAdmin.StartConsumerAsync(consumerName);

                                    await context.Response.WriteAsync(string.Empty);
                                    await context.Response.CompleteAsync();
                                });

                            routeBuilder.MapPut(
                                "/consumers/{name}/restart",
                                async context =>
                                {
                                    var consumerAdmin = context.RequestServices.GetRequiredService<IConsumerAdmin>();

                                    var consumerName = context.Request.RouteValues["name"].ToString();

                                    await consumerAdmin.RestartConsumerAsync(consumerName);

                                    await context.Response.WriteAsync(string.Empty);
                                    await context.Response.CompleteAsync();
                                });

                            routeBuilder.MapPut(
                                "/consumers/{name}/changeWorkers/{workersCount}",
                                async context =>
                                {
                                    var consumerAdmin = context.RequestServices.GetRequiredService<IConsumerAdmin>();

                                    var consumerName = context.Request.RouteValues["name"].ToString();
                                    var workersCount = Convert.ToInt32(context.Request.RouteValues["workersCount"]);

                                    await consumerAdmin.ChangeWorkersCountAsync(consumerName, workersCount);

                                    await context.Response.WriteAsync(string.Empty);
                                    await context.Response.CompleteAsync();
                                });

                            routeBuilder.MapGet(
                                "/consumers/telemetry",
                                async context =>
                                {
                                    var telemetryStorage = context.RequestServices.GetRequiredService<ITelemetryStorage>();

                                    var response = telemetryStorage.Get().Adapt();
                                    var responseJson = JsonConvert.SerializeObject(response, jsonSettings);

                                    context.Response.StatusCode = 200;

                                    await context.Response.WriteAsync(responseJson);
                                    await context.Response.CompleteAsync();
                                });

                            configuration.EndpointHandler?.Invoke(endpoint);
                        });
                });

            return app;
        }
    }
}
