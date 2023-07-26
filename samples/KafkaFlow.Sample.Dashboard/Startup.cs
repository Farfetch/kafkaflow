namespace KafkaFlow.Sample.Dashboard;

using KafkaFlow.Admin.Dashboard;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class Startup
{
    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddKafkaFlowHostedService(
            kafka => kafka
                .UseConsoleLog()
                .AddCluster(
                    cluster =>
                    {
                        const string topicName = "topic-dashboard";
                        cluster
                            .WithBrokers(new[] { "localhost:9092" })
                            .EnableAdminMessages("kafkaflow.admin", "kafkaflow.admin.group.id")
                            .EnableTelemetry("kafkaflow.admin", "kafkaflow.telemetry.group.id")
                            .CreateTopicIfNotExists(topicName, 3, 1)
                            .AddConsumer(
                                consumer =>
                                {
                                    consumer
                                        .Topics(topicName)
                                        .WithGroupId("groupid-dashboard")
                                        .WithName("consumer-dashboard")
                                        .WithBufferSize(100)
                                        .WithWorkersCount(20)
                                        .WithAutoOffsetReset(AutoOffsetReset.Latest);
                                });
                    })
        );

        services
            .AddControllers();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
    {
        app
            .UseRouting()
            .UseEndpoints(endpoints => { endpoints.MapControllers(); })
            .UseKafkaFlowDashboard();
    }
}