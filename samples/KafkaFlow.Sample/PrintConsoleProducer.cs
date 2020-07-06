namespace KafkaFlow.Sample
{
    using System;
    using KafkaFlow.Producers;

    public class PrintConsoleProducer
    {
        private readonly IMessageProducer<PrintConsoleProducer> producer;

        public PrintConsoleProducer(IMessageProducer<PrintConsoleProducer> producer)
        {
            this.producer = producer;
        }

        public void Produce(object message)
        {
            this.producer.Produce(
                Guid.NewGuid().ToString(),
                message);
        }
    }
}
