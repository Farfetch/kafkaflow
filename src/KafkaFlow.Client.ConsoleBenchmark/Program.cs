namespace KafkaFlow.Client.ConsoleBenchmark
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using KafkaFlow.Client.Producers;
    using KafkaFlow.Client.Producers.Partitioners;
    using KafkaFlow.Client.Protocol;
    using KafkaFlow.Client.Protocol.Messages;

    class Program
    {
        static async Task Main(string[] args)
        {
            var cluster = new KafkaCluster(
                new[] { new BrokerAddress("localhost", 9092) },
                "test-id",
                TimeSpan.FromSeconds(5));

            var producer = new Producer(
                cluster,
                new ProducerConfiguration
                {
                    Acks = ProduceAcks.All,
                    ProduceTimeout = TimeSpan.FromSeconds(10),
                    MaxProduceBatchSize = 500,
                    Linger = TimeSpan.FromMilliseconds(30)
                },
                new ByteSumPartitioner());

            var header = new Headers
            {
                ["test_header"] = Encoding.UTF8.GetBytes("header_value")
            };

            var tasks = Enumerable
                .Range(0, 20)
                .Select(
                    x => producer.ProduceAsync(
                        new ProduceData(
                            "test-client",
                            Encoding.UTF8.GetBytes($"teste_key_{Guid.NewGuid()}"),
                            Encoding.UTF8.GetBytes("teste_value"),
                            header)));


            await Task.WhenAll(tasks);

            Console.WriteLine("Starting...");


            var sw = Stopwatch.StartNew();

            JetBrains.Profiler.Api.MeasureProfiler.StartCollectingData();

            tasks = Enumerable
                .Range(0, 100000)
                .Select(
                    x => producer.ProduceAsync(
                        new ProduceData(
                            "test-client",
                            Encoding.UTF8.GetBytes($"teste_key_{Guid.NewGuid()}"),
                            Encoding.UTF8.GetBytes("teste_value"),
                            header)))
                .AsParallel()
                .ToList();
            
            var results = await Task.WhenAll(tasks);

            JetBrains.Profiler.Api.MeasureProfiler.SaveData();

            sw.Stop();

            Console.WriteLine("Ended! Elapsed: {0}ms", sw.ElapsedMilliseconds);
        }
    }
}
