namespace KafkaFlow.Client.Sample
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using KafkaFlow.Client.Producers;

    class Program
    {
        static async Task Main(string[] args)
        {
            var producer = ProducerBuilder.CreateProducer();

            var tasks = Enumerable
                .Range(0, 100)
                .Select(
                    x => producer.ProduceAsync(
                        new ProduceData(
                            "test-client",
                            Encoding.UTF8.GetBytes($"teste_key_{Guid.NewGuid()}"),
                            Encoding.UTF8.GetBytes("teste_value"))));

            var results = await Task.WhenAll(tasks);
        }
    }
}
