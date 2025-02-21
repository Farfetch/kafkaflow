using System;
using System.Threading;
using System.Threading.Tasks;
using KafkaFlow.Producers;
using Microsoft.Extensions.Hosting;

namespace KafkaFlow.Sample.CooperativeSticky;

public class HostedService : IHostedService
{
    private IMessageProducer _producer;
    const string producerName = "PrintConsole";
    const string topicName = "sample-topic";


    public HostedService(IProducerAccessor producerAccessor)
    {
        _producer = producerAccessor.GetProducer(producerName);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (true)
            {
                await _producer.ProduceAsync(
                    topicName,
                    Guid.NewGuid().ToString(),
                    new TestMessage { Text = $"Message: {Guid.NewGuid()}" });
                await Task.Delay(500, cancellationToken);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}