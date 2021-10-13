namespace KafkaFlow.Client.ConsoleBenchmark
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using KafkaFlow.Client.Producers;
    using KafkaFlow.Client.Protocol;

    class Program
    {
        static async Task Main(string[] args)
        {
            var metricReader = MetricsReaderBuilder.BuildLagReader(
                new[] { new BrokerAddress("localhost", 9092) },
                timeout: TimeSpan.FromSeconds(100));

            Console.WriteLine("Starting...");

            var sw = Stopwatch.StartNew();

            JetBrains.Profiler.Api.MeasureProfiler.StartCollectingData();

            var tasks = Enumerable
                .Range(0, 100000)
                .Select(_ => metricReader.GetLagAsync("test-client", "print-console-handler-1"))
                .ToList();

            await Task.WhenAll(tasks);

            JetBrains.Profiler.Api.MeasureProfiler.SaveData();

            sw.Stop();

            Console.WriteLine("Ended! Elapsed: {0}ms", sw.ElapsedMilliseconds);
            //Console.Read();
        }
    }
}
