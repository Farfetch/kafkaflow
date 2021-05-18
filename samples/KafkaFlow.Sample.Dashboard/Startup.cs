namespace KafkaFlow.Sample.Dashboard
{
    using KafkaFlow.Admin.Dashboard;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    
    public class Startup
    {

        public IKafkaBus bus { get; set; }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddKafka(
                kafka => kafka
                    .UseConsoleLog()
                    .AddCluster(
                        cluster => cluster
                            .WithBrokers(new[] { "localhost:9092" })
                            .EnableAdminMessages("kafka-flow.admin", "kafka-flow.admin.group.id")
                            .EnableTelemetry("kafka-flow.telemetry", "kafka-flow.telemetry.group.id")
                            .AddConsumer(
                                consumer => consumer
                                    .Topics("topic-dashboard")
                                    .WithGroupId("groupid-dashboard")
                                    .WithName("consumer-dashboard")
                                    .WithBufferSize(100)
                                    .WithWorkersCount(20)
                                    .WithAutoOffsetReset(AutoOffsetReset.Latest)
                            ))
            );

            services
                .AddKafkaFlowDashboard()
                .AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
        {
            app
                .UseRouting()
                .UseEndpoints(endpoints => { endpoints.MapControllers(); })
                .UseKafkaFlowDashboard(env);

            lifetime.ApplicationStarted.Register(() =>
            {
                bus = app.ApplicationServices.CreateKafkaBus();
                bus.StartAsync(lifetime.ApplicationStopped);
            });
            
            lifetime.ApplicationStopping.Register(() => bus.StopAsync().GetAwaiter().GetResult());
        }
    }
}
