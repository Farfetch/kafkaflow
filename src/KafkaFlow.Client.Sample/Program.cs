namespace KafkaFlow.Client.Sample
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using KafkaFlow.Client.Producers;

    class Program
    {
        static async Task Main(string[] args)
        {
            var producer = ProducerBuilder.CreateProducer();

            var result = await producer.ProduceAsync(
                new ProduceData(
                    "test-client",
                    Encoding.UTF8.GetBytes($"teste_key_{Guid.NewGuid()}"),
                    Encoding.UTF8.GetBytes("teste_value")));

            Console.WriteLine("Starting...");

            var sw = Stopwatch.StartNew();

            var tasks = Enumerable
                .Range(0, 100000)
                .Select(
                    x => producer.ProduceAsync(
                        new ProduceData(
                            "test-client",
                            Encoding.UTF8.GetBytes($"teste_key_{Guid.NewGuid()}"),
                            Encoding.UTF8.GetBytes("teste_value"))));

            var results = await Task.WhenAll(tasks);

            sw.Stop();

            Console.WriteLine("Ended! Elapsed: {0}ms", sw.ElapsedMilliseconds);
            Thread.Sleep(5000);
        }
    }
}
