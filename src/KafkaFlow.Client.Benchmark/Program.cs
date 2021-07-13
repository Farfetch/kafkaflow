namespace KafkaFlow.Client.Benchmark
{
    using BenchmarkDotNet.Running;

    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<KafkaProducerBenchmark>();
        }
    }
}
