namespace KafkaFlow.Client.Benchmark
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using BenchmarkDotNet.Attributes;
    using Confluent.Kafka;
    using KafkaFlow.Client.Producers;
    using KafkaFlow.Client.Producers.Partitioners;
    using KafkaFlow.Client.Protocol;

    [MemoryDiagnoser]
    [StopOnFirstError]
    [ShortRunJob]
    // [SimpleJob(launchCount: 1, warmupCount: 1, targetCount: 1, invocationCount: 10)]
    public class KafkaProducerBenchmark
    {
        private readonly IProducer kafkaFlowProducer;

        private readonly IProducer<byte[], byte[]> confluentProducer;

        public KafkaProducerBenchmark()
        {
            var cluster = new KafkaCluster(
                new[] { new BrokerAddress("localhost", 9092) },
                "test-id",
                TimeSpan.FromSeconds(5));

            this.kafkaFlowProducer = new Producer(
                cluster,
                new ProducerConfiguration
                {
                    Acks = ProduceAcks.All,
                    ProduceTimeout = TimeSpan.FromSeconds(10),
                    MaxProduceBatchSize = 25000,
                    Linger = TimeSpan.FromMilliseconds(5)
                },
                new ByteSumPartitioner());

            this.confluentProducer = new ProducerBuilder<byte[], byte[]>(
                    new ProducerConfig
                    {
                        Acks = Acks.All,
                        LingerMs = 5,
                        BatchNumMessages = 25000,
                        BootstrapServers = "localhost",
                    })
                .Build();
        }

        [Benchmark]
        public async Task KafkaFlow_1_Message()
        {
            await this.ProduceKafkaFlow();
        }

        [Benchmark]
        public async Task Confluent_1_Message()
        {
            await this.ProduceConfluent();
        }

        [Benchmark]
        public async Task KafkaFlow_100_Messages()
        {
            await Task.WhenAll(Enumerable.Range(0, 100).Select(x => this.ProduceKafkaFlow()));
        }

        [Benchmark]
        public async Task Confluent_100_Messages()
        {
            await Task.WhenAll(Enumerable.Range(0, 100).Select(x => this.ProduceConfluent()));
        }

        // [Benchmark]
        // public async Task KafkaFlow_1000_Messages()
        // {
        //     await Task.WhenAll(Enumerable.Range(0, 1000).Select(x => this.ProduceKafkaFlow()));
        // }
        //
        // [Benchmark]
        // public async Task Confluent_1000_Messages()
        // {
        //     await Task.WhenAll(Enumerable.Range(0, 1000).Select(x => this.ProduceConfluent()));
        // }

        // [Benchmark]
        // public async Task KafkaFlow_10000_Messages()
        // {
        //     await Task.WhenAll(Enumerable.Range(0, 10000).Select(x => this.ProduceKafkaFlow()));
        // }
        //
        // [Benchmark]
        // public async Task Confluent_10000_Messages()
        // {
        //     await Task.WhenAll(Enumerable.Range(0, 10000).Select(x => this.ProduceConfluent()));
        // }
        //
        // [Benchmark]
        // public async Task KafkaFlow_100000_Messages()
        // {
        //     await Task.WhenAll(Enumerable.Range(0, 100000).Select(x => this.ProduceKafkaFlow()));
        // }
        //
        // [Benchmark]
        // public async Task Confluent_100000_Messages()
        // {
        //     await Task.WhenAll(Enumerable.Range(0, 100000).Select(x => this.ProduceConfluent()));
        // }

        private Task ProduceKafkaFlow()
        {
            var header = new KafkaFlow.Client.Protocol.Messages.Headers()
            {
                ["test_header"] = Encoding.UTF8.GetBytes("header_value"),
            };

            return this.kafkaFlowProducer.ProduceAsync(
                new ProduceData(
                    "test-client",
                    Encoding.UTF8.GetBytes($"teste_key_{Guid.NewGuid()}"),
                    Encoding.UTF8.GetBytes("teste_value"),
                    header));
        }

        private Task ProduceConfluent()
        {
            var header = new Confluent.Kafka.Headers
            {
                { "test_header", Encoding.UTF8.GetBytes("header_value") },
            };

            return this.confluentProducer.ProduceAsync(
                "test-client",
                new Message<byte[], byte[]>
                {
                    Key = Encoding.UTF8.GetBytes($"teste_key_{Guid.NewGuid()}"),
                    Value = Encoding.UTF8.GetBytes("teste_value"),
                    Headers = header,
                });
        }
    }
}
