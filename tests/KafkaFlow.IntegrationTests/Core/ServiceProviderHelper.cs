using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using KafkaFlow.Configuration;
using KafkaFlow.IntegrationTests.Core.Producers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Polly;

namespace KafkaFlow.IntegrationTests.Core;

public class ServiceProviderHelper
{
    private bool _isPartitionAssigned;
    private IKafkaBus _bus;

    public async Task<IServiceProvider> GetServiceProviderAsync(
        Action<IConsumerConfigurationBuilder> consumerConfiguration,
        Action<IProducerConfigurationBuilder> producerConfiguration,
        Action<IClusterConfigurationBuilder> builderConfiguration = null,
        Action<IGlobalEvents> configureGlobalEvents = null)
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
                    builder.WithPartitionsAssignedHandler((_, _) => { _isPartitionAssigned = true; });
                });
            }

            if (producerConfiguration != null)
            {
                cluster.AddProducer<JsonProducer2>(producerConfiguration);
            }
        };

        clusterBuilderAction += (_, cluster) => { builderConfiguration?.Invoke(cluster); };

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
                    kafka =>
                    {
                        kafka
                            .UseLogHandler<TraceLogHandler>()
                            .AddCluster(cluster => { clusterBuilderAction.Invoke(context, cluster); });

                        if (configureGlobalEvents != null)
                        {
                            kafka.SubscribeGlobalEvents(configureGlobalEvents);
                        }
                    }))
            .UseDefaultServiceProvider(
                (_, options) =>
                {
                    options.ValidateScopes = true;
                    options.ValidateOnBuild = true;
                });

        var host = builder.Build();
        _bus = host.Services.CreateKafkaBus();
        await _bus.StartAsync();

        await WaitForPartitionAssignmentAsync();

        return host.Services;
    }

    public async Task StopBusAsync()
    {
        await _bus.StopAsync();
    }

    private async Task WaitForPartitionAssignmentAsync()
    {
        await Policy
            .HandleResult<bool>(isAvailable => !isAvailable)
            .WaitAndRetryAsync(Enumerable.Range(0, 6).Select(i => TimeSpan.FromSeconds(Math.Pow(i, 2))))
            .ExecuteAsync(() => Task.FromResult(_isPartitionAssigned));
    }
}
