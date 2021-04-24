namespace KafkaFlow.IntegrationTests.Core.Handlers
{
    using System.Threading.Tasks;
    using KafkaFlow.IntegrationTests.Core.Messages;
    using KafkaFlow.Producers;
    using KafkaFlow.TypedHandler;

    internal class StreamingMessageHandler : IMessageHandler<TestMessage3>
    {
        private readonly IMessageProducer producer;

        public StreamingMessageHandler(IProducerAccessor producers)
        {
            this.producer = producers["JsonOtboundProducer"];
        }

        public async Task Handle(IMessageContext context, TestMessage3 message)
        {
            using (var transactionScope = new ConsumerProducerTransactionScope(this.producer, context))
            {
                var outboundMessage = new TestMessage4
                {
                    Id = message.Id,
                    Value = message.Value,
                    OtherValue = message.Value,
                    Version = message.Version,
                };

                await this.producer.ProduceAsync(outboundMessage.Id.ToString(), outboundMessage).ConfigureAwait(false);
                transactionScope.Complete();
            }

            MessageStorage.Add(message);
        }
    }
}
