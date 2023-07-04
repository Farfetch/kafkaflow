namespace KafkaFlow.Sample.Dashboard;

using System;
using System.Threading.Tasks;
using KafkaFlow.Admin.Dashboard;
using KafkaFlow.Clusters;
using KafkaFlow.Consumers;
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
                                            .WithBufferSize(20)
                                            .WithManualStoreOffsets()
                                            .WithWorkersCount(
                                                (context, resolver) =>
                                                    new ConsumerLagWorkerBalancer(
                                                            resolver.Resolve<IClusterManager>(),
                                                            resolver.Resolve<IConsumerAccessor>(),
                                                            2,
                                                            1)
                                                        .GetWorkersCountAsync(context),
                                                TimeSpan.FromSeconds(60))
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
        Console.WriteLine("message consumed {0}", context.Message.Key.GetHashCode());
        _ = Task.Delay(100).ContinueWith(_ => context.ConsumerContext.StoreOffset());
        await Task.CompletedTask;
    }
}
