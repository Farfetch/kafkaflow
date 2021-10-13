namespace KafkaFlow.Client.Sample
{
    using System;
    using System.Threading.Tasks;
    using KafkaFlow.Client.Producers;
    using KafkaFlow.Client.Protocol;

    class Program
    {
        static async Task Main(string[] args)
        {
            var metricReader = MetricsReaderBuilder.BuildLagReader(new[] { new BrokerAddress("localhost", 9092) });

            var lag = await metricReader.GetLagAsync("test-client", "print-console-handler-1");

            Console.Write($"LAG: {lag}");
        }
    }
}
