namespace KafkaFlow.Client.ConsoleBenchmark
{
    using System;
    using System.Diagnostics;
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
            var metricReader = MetricsReaderBuilder.BuildLagReader(
                new[] { new BrokerAddress("localhost", 9092) },
                timeout: TimeSpan.FromSeconds(10));

            Console.WriteLine("Starting...");

            var sw = Stopwatch.StartNew();

            JetBrains.Profiler.Api.MeasureProfiler.StartCollectingData();

            var tasks = Enumerable
                .Range(0, 10000)
                .Select(_ => metricReader.GetLagAsync("test-topic", "print-console-handler"))
                .ToList();

            await Task.WhenAll(tasks);

            JetBrains.Profiler.Api.MeasureProfiler.SaveData();

            sw.Stop();

            Console.WriteLine("Ended! Elapsed: {0}ms", sw.ElapsedMilliseconds);
        }
    }
}
