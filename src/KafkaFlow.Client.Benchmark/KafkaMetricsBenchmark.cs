namespace KafkaFlow.Client.Benchmark
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using BenchmarkDotNet.Attributes;
    using KafkaFlow.Client.Metrics;
    using KafkaFlow.Client.Protocol;

    [MemoryDiagnoser]
    [StopOnFirstError]
    [RankColumn]
    [MinColumn]
    [MaxColumn]
    [Q1Column]
    [Q3Column]
    [AllStatisticsColumn]
    [ShortRunJob]
    [MarkdownExporterAttribute.GitHub]
    [GcServer(true)]
    public class KafkaMetricsBenchmark
    {
        private readonly ILagReader lagReader;

        [Params(1, 10, 100, 1000, 10000)]
        public int MaxCall { get; set; }

        public KafkaMetricsBenchmark()
        {
            this.lagReader = MetricsReaderBuilder
                .BuildLagReader(
                    new[] { new BrokerAddress("localhost", 9092) },
                    timeout: TimeSpan.FromSeconds(100));
        }

        [Benchmark]
        public async Task Benchmark()
        {
            await Task.WhenAll(Enumerable.Range(0, this.MaxCall).Select(x => this.lagReader.GetLagAsync("test-topic", "print-console-handler")));
        }
    }
}
