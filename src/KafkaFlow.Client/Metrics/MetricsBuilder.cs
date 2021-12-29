namespace KafkaFlow.Client.Metrics
{
    using System;
    using KafkaFlow.Client.Protocol;

    public class MetricsBuilder
    {
        public static IMetricReader CreateReader()
        {
            var cluster = new KafkaCluster(
                new[] { new BrokerAddress("localhost", 9092) },
                "test-id",
                TimeSpan.FromSeconds(5));

            return new MetricReader(cluster);
        }
    }
}
