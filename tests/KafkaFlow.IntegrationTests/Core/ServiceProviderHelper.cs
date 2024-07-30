using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using KafkaFlow.Configuration;
using KafkaFlow.IntegrationTests.Core.Producers;
using KafkaFlow.Serializer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Polly;

namespace KafkaFlow.IntegrationTests.Core;

public class ServiceProviderHelper
{
    private bool _isPartitionAssigned;

    public async Task<IServiceProvider> GetServiceProviderAsync(
        Action<IConsumerConfigurationBuilder> consumerConfiguration,
        Action<IProducerConfigurationBuilder> producerConfiguration,
        Action<IClusterConfigurationBuilder> builderConfiguration = null)
    {
        if (consumerConfiguration == null && producerConfiguration == null)
        {
            throw new ArgumentException("At least one of the configurations must be provided");
        }
        
        var clusterBuilderAction = (HostBuilderContext context, IClusterConfigurationBuilder cluster) =>
        {
            cluster.WithBrokers(context.Configuration.GetValue<string>("Kafka:Brokers").Split(';'));
            
            if (consumerConfiguration != null)
            {
                cluster.AddConsumer(builder =>
                {
                    consumerConfiguration(builder);
                    builder
                        .WithBufferSize(100)
                        .WithWorkersCount(10)
                        .WithPartitionsAssignedHandler((_, _) =>
                        {
                            _isPartitionAssigned = true;
                        })
                        .AddMiddlewares(middlewares => middlewares.AddDeserializer<JsonCoreDeserializer>());
                });
            }
            
            if (producerConfiguration != null)
            {
                cluster.AddProducer<JsonProducer>(producerConfiguration);
            }
        };

        clusterBuilderAction += (_, cluster) =>
        {
            builderConfiguration?.Invoke(cluster);
        };

        var builder = Host
            .CreateDefaultBuilder()
            .ConfigureAppConfiguration(
                (_, config) =>
                {
                    config
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile(
                            "conf/appsettings.json",
                            false,
                            true)
                        .AddEnvironmentVariables();
                })
            .ConfigureServices((context, services) =>
                services.AddKafka(
                    kafka => kafka
                        .UseLogHandler<TraceLogHandler>()
                        .AddCluster(cluster => { clusterBuilderAction.Invoke(context, cluster); })))
            .UseDefaultServiceProvider(
                (_, options) =>
                {
                    options.ValidateScopes = true;
                    options.ValidateOnBuild = true;
                });

        var host = builder.Build();
        var bus = host.Services.CreateKafkaBus();
        await bus.StartAsync();
        
        await WaitForPartitionAssignmentAsync();

        return host.Services;
    }
    
    private async Task WaitForPartitionAssignmentAsync()
    {
        await Policy
            .HandleResult<bool>(isAvailable => !isAvailable)
            .WaitAndRetryAsync(Enumerable.Range(0, 6).Select(i => TimeSpan.FromSeconds(Math.Pow(i, 2))))
            .ExecuteAsync(() => Task.FromResult(_isPartitionAssigned));
    }
}