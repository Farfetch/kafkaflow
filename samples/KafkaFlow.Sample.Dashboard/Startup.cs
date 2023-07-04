namespace KafkaFlow.Sample.Dashboard;

using System;
using System.Threading.Tasks;
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
        services
            .AddSingleton<ConsumerLagWorkersCountCalculator>()
            .AddKafkaFlowHostedService(
                kafka => kafka
                    .UseConsoleLog()
                    .AddCluster(
                        cluster =>
                        {
                            const string topicName = "topic-dashboard";
                            cluster
                                .WithBrokers(new[] { "localhost:9092" })
                                .EnableAdminMessages("kafka-flow.admin", "kafka-flow.admin.group.id")
                                .EnableTelemetry("kafka-flow.admin", "kafka-flow.telemetry.group.id")
                                .CreateTopicIfNotExists(topicName, 3, 1)
                                .AddConsumer(
                                    consumer =>
                                    {
                                        consumer
                                            .Topics(topicName)
                                            .WithGroupId("groupid-dashboard")
                                            .WithName("consumer-dashboard")
                                            .WithBufferSize(1)
                                            .WithWorkersCount(
                                                (context, resolver) =>
                                                    resolver.Resolve<ConsumerLagWorkersCountCalculator>().CalculateAsync(context))
                                            .WithWorkersCountEvaluationInterval(TimeSpan.FromSeconds(60))
                                            .WithAutoOffsetReset(AutoOffsetReset.Latest)
                                            .AddMiddlewares(
                                                m =>
                                                    m.Add<DelayMiddleware>());
                                    })
                                .AddProducer("producer", producer => producer.DefaultTopic(topicName));
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

public class DelayMiddleware : IMessageMiddleware
{
    public async Task Invoke(IMessageContext context, MiddlewareDelegate next)
    {
        await Task.Delay(1000);
    }
}
