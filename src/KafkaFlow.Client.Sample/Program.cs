namespace KafkaFlow.Client.Sample
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using KafkaFlow.Client.Exceptions;
    using KafkaFlow.Client.Producers;

    class Program
    {
        static async Task Main(string[] args)
        {
            var producer = ProducerBuilder.CreateProducer();

            string input;

            do
            {
                Console.Write("Messages to produce or exit: ");
                input = Console.ReadLine();

                if (!int.TryParse(input, out var messageCount))
                {
                    continue;
                }

                try
                {
                    Console.WriteLine("Starting...");

                    var sw = Stopwatch.StartNew();

                    var header = new Headers
                    {
                        ["test_header"] = Encoding.UTF8.GetBytes("header_value")
                    };

                    var tasks = Enumerable
                        .Range(0, messageCount)
                        .Select(
                            x => producer.ProduceAsync(
                                new ProduceData(
                                    "test-client",
                                    Encoding.UTF8.GetBytes($"teste_key_{Guid.NewGuid()}"),
                                    Encoding.UTF8.GetBytes("teste_value"),
                                    header)));


                    var results = await Task.WhenAll(tasks);

                    sw.Stop();

                    Console.WriteLine("Ended! Elapsed: {0}ms", sw.ElapsedMilliseconds);
                }
                catch (ProduceException e)
                {
                    Console.WriteLine("Error code:" + e.ErrorCode);
                    Console.WriteLine("Message : " + e.Message);
                    Console.WriteLine(e.RecordMessage);
                }
            } while (input != "exit");
        }
    }
}
