namespace KafkaFlow.Samples.Common
{
    using System;
    using System.Threading.Tasks;
    using KafkaFlow.Producers;

    public class PrintConsoleProtobufProducer
    {
        private readonly IMessageProducer<PrintConsoleProtobufProducer> producer;

        public PrintConsoleProtobufProducer(IMessageProducer<PrintConsoleProtobufProducer> producer)
        {
            this.producer = producer;
        }

        public Task ProduceAsync(object message)
        {
            return this.producer.ProduceAsync(
                Guid.NewGuid().ToString(),
                message);
        }
    }
}
