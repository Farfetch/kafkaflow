namespace KafkaFlow.Client.Producers
{
    using System;
    using KafkaFlow.Client.Protocol;

    public class ProducerConfiguration
    {
        public ProduceAcks Acks { get; set; }
        public TimeSpan Timeout { get; set; }
        public int MaxProduceBatchSize { get; set; }
    }
}
