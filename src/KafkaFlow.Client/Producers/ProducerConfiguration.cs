namespace KafkaFlow.Client.Producers
{
    using System;
    using KafkaFlow.Client.Protocol;

    public class ProducerConfiguration
    {
        public ProduceAcks Acks { get; set; }
        public TimeSpan ProduceTimeout { get; set; }

        public TimeSpan Linger { get; set; }
        public int MaxProduceBatchSize { get; set; }
    }
}
