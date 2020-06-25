namespace KafkaFlow.Samples.Common
{
    using System;
    using System.Threading.Tasks;
    using KafkaFlow.Producers;

    public class PrintConsoleJsonProducer
    {
        private readonly IMessageProducer<PrintConsoleJsonProducer> producer;

        public PrintConsoleJsonProducer(IMessageProducer<PrintConsoleJsonProducer> producer)
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
