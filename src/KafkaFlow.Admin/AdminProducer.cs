namespace KafkaFlow.Admin
{
    using System;
    using System.Threading.Tasks;
    using KafkaFlow.Admin.Messages;

    internal class AdminProducer : IAdminProducer
    {
        private readonly IMessageProducer<AdminProducer> producer;
        private readonly int topicPartition;

        public AdminProducer(IMessageProducer<AdminProducer> producer, int topicPartition)
        {
            this.producer = producer;
            this.topicPartition = topicPartition;
        }

        public Task ProduceAsync(IAdminMessage message) =>
            this.producer.ProduceAsync(Guid.NewGuid().ToString(), message, partition: this.topicPartition);
    }
}
