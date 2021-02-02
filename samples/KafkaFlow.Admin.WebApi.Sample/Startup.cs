using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace KafkaFlow.Admin.WebApi.Sample
{
    using global::Microsoft.OpenApi.Models;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddKafka(
                kafka => kafka
                    .UseConsoleLog()
                    .AddCluster(
                        cluster => cluster
                            .WithBrokers(new[] { "localhost:9092" })
                            .EnableAdminMessages("kafka-flow.admin", Guid.NewGuid().ToString()))
            );

            services
                .AddSwaggerGen(
                    c =>
                    {
                        c.SwaggerDoc(
                            "kafka-flow",
                            new OpenApiInfo
                            {
                                Title = "KafkaFlow Admin",
                                Version = "kafka-flow",
                            });
                        c.SwaggerDoc(
                            "v1",
                            new OpenApiInfo
                            {
                                Title = "Api v1",
                                Version = "v1",
                            });
                        c.DocInclusionPredicate((docName, apiDesc) =>
                        {
                            return apiDesc.RelativePath.StartsWith(docName.Replace("_", @"/"));
                        });
                    })
                .AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            app
                .UseSwagger()
                .UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/kafka-flow/swagger.json", "KafkaFlow Admin");
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Api v1");
                });

            var kafkaBus = app.ApplicationServices.CreateKafkaBus();

            lifetime.ApplicationStarted.Register(() => kafkaBus.StartAsync(lifetime.ApplicationStopped));
        }
    }
}
