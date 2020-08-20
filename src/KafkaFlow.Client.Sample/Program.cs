namespace KafkaFlow.Client.Sample
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using KafkaFlow.Client.Metrics;
    using KafkaFlow.Client.Protocol;

    class Program
    {
        protected Program()
        {
        }

        static async Task Main(string[] args)
        {
            var metricReader = MetricsReaderBuilder.BuildLagReader(new[] { new BrokerAddress("localhost", 9092) });

            var lag = await metricReader.GetLagAsync("test-topic", "print-console-handler");

            Console.Write(string.Join("\n", lag.Select(l => $"Partition: {l.Partition}. Lag: {l.Lag}")));
        }
    }
}
