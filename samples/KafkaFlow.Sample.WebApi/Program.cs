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
                .EnableAdminMessages("kafkaflow.admin")
        )
);

builder.Services
    .AddSwaggerGen(
        c =>
        {
            c.SwaggerDoc(
                "kafkaflow",
                new OpenApiInfo
                {
                    Title = "KafkaFlow Admin",
                    Version = "kafkaflow",
                });
        })
    .AddControllers();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/kafkaflow/swagger.json", "KafkaFlow Admin");
});

var kafkaBus = app.Services.CreateKafkaBus();
await kafkaBus.StartAsync();

await app.RunAsync();
