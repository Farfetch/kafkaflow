namespace KafkaFlow.Admin
{
    using System;
    using System.Threading.Tasks;
    using KafkaFlow.Admin.Messages;
    using KafkaFlow.Producers;

    internal class AdminProducer : IAdminProducer
    {
        private readonly IMessageProducer<AdminProducer> producer;

        public AdminProducer(IMessageProducer<AdminProducer> producer) => this.producer = producer;

        public Task ProduceAsync(IAdminMessage message) =>
            this.producer.ProduceAsync(Guid.NewGuid().ToString(), message);
    }
}
