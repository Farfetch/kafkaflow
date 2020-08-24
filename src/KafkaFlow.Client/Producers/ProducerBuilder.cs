namespace KafkaFlow.Client.Producers
{
    using System;
    using KafkaFlow.Client.Producers.Partitioners;
    using KafkaFlow.Client.Protocol;

    public class ProducerBuilder
    {
        public static IProducer CreateProducer()
        {
            var cluster = new KafkaCluster(
                new[] { new KafkaHostAddress("localhost", 9092) },
                "test-id",
                TimeSpan.FromSeconds(5));

            return new Producer(
                cluster,
                new ProducerConfiguration
                {
                    Acks = ProduceAcks.All,
                    ProduceTimeout = TimeSpan.FromSeconds(10),
                    MaxProduceBatchSize = 25000,
                    Linger = TimeSpan.FromMilliseconds(10)
                },
                new ByteSumPartitioner());
        }
    }
}
