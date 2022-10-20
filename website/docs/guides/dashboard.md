---
sidebar_position: 5
---

# Dashboard

KafkaFlow provides a Dashboard where you can visualize information related to your consumers and make use of all operations available on [KafkaFlow.Admin.WebApi](https://github.com/Farfetch/kafka-flow/wiki/admin). 

It is important to note that the Dashboard runs, shows information, and manages the consumers on all application instances, which means if you have 10 machines running your application, the dashboard will be running in every instance and any operation will affect the consumer in all the machines.

## Configuring
Install the following packages:
* [KafkaFlow.Admin](https://www.nuget.org/packages/KafkaFlow.Admin/)
* [KafkaFlow.Admin.WebApi](https://www.nuget.org/packages/KafkaFlow.Admin.WebApi/)
* [KafkaFlow.Admin.Dashboard](https://www.nuget.org/packages/KafkaFlow.Admin.Dashboard/)

```csharp
 public void ConfigureServices(IServiceCollection services)
 {
    services.AddKafka(kafka => kafka
        .AddCluster(cluster => cluster
            .WithBrokers(new[] { "localhost:9092" })
            .EnableAdminMessages("kafka-flow.admin")
            .EnableTelemetry("kafka-flow.admin") // you can use the same topic used in EnableAdminMessages, if need it
        ));
 }

public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
{
    app
        .UseRouting()
        .UseEndpoints(endpoints => { endpoints.MapControllers(); })
        .UseKafkaFlowDashboard();
    ...
}
```

## Accessing
The dashboard UI is available at `/kafka-flow` and is refreshed every 5 seconds with telemetry data available at the endpoint `/kafka-flow/telemetry`. 

![image](https://user-images.githubusercontent.com/233064/124478023-1d773680-dd7b-11eb-89e4-41a1f4f36a6f.png)

