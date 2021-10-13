namespace KafkaFlow.Client.Benchmark
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using BenchmarkDotNet.Attributes;
    using KafkaFlow.Client.Metrics;
    using KafkaFlow.Client.Producers;
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

        public KafkaMetricsBenchmark()
        {
            this.lagReader = MetricsReaderBuilder.BuildLagReader(
                new[] { new BrokerAddress("localhost", 9092) },
                timeout: TimeSpan.FromSeconds(100));
        }

        private Task GetLagAsync()
        {
            return this.lagReader.GetLagAsync("test-client", "print-console-handler-1");
        }

        [Benchmark]
        public async Task Lag_1_Call()
        {
            await this.GetLagAsync();
        }

        [Benchmark]
        public async Task Lag_10_Call()
        {
            await Task.WhenAll(Enumerable.Range(0, 10).Select(x => this.GetLagAsync()));
        }

        [Benchmark]
        public async Task Lag_100_Call()
        {
            await Task.WhenAll(Enumerable.Range(0, 100).Select(x => this.GetLagAsync()));
        }

        [Benchmark]
        public async Task Lag_1000_Call()
        {
            await Task.WhenAll(Enumerable.Range(0, 1000).Select(x => this.GetLagAsync()));
        }

        [Benchmark]
        public async Task Lag_10000_Call()
        {
            await Task.WhenAll(Enumerable.Range(0, 10000).Select(x => this.GetLagAsync()));
        }

        //[Benchmark]
        public async Task Lag_100000_Call()
        {
            await Task.WhenAll(Enumerable.Range(0, 100000).Select(x => this.GetLagAsync()));
        }
    }
}
