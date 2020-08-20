namespace KafkaFlow.Client.Benchmark
{
    using BenchmarkDotNet.Running;

    class Program
    {
        protected Program()
        {
        }

        static void Main(string[] args)
        {
            BenchmarkRunner.Run<KafkaMetricsBenchmark>();
        }
    }
}
