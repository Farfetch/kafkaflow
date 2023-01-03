using KafkaFlow;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddKafka(
    kafka => kafka
        .UseConsoleLog()
        .AddCluster(
            cluster => cluster
                .WithBrokers(new[] { "localhost:9092" })
                .EnableAdminMessages("kafka-flow.admin")
        )
);

builder.Services
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
        })
    .AddControllers();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/kafka-flow/swagger.json", "KafkaFlow Admin");
});

var kafkaBus = app.Services.CreateKafkaBus();
await kafkaBus.StartAsync();

await app.RunAsync();
        