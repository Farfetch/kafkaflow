namespace KafkaFlow.Producers
{
    using System;

    /// <summary>
    /// Consumer/Producer transaction scope
    /// </summary>
    public class ConsumerProducerTransactionScope : IDisposable
    {
        private readonly IConsumerContext consumerContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsumerProducerTransactionScope"/> class.
        /// Constructor
        /// </summary>
        /// <param name="producer">Producer</param>
        /// <param name="messageContext">Message context</param>
        public ConsumerProducerTransactionScope(IMessageProducer producer, IMessageContext messageContext)
        {
            this.consumerContext = messageContext.ConsumerContext;
            producer.RegisterConsumerProducerTransaction(this.consumerContext);
        }

        /// <summary>
        /// Complete transaction scope
        /// </summary>
        /// <exception cref="InvalidOperationException">On committing before beginning a transaction</exception>
        public void Complete()
        {
            this.consumerContext.StoreOffset();
        }

        /// <inheritdoc />
        public void Dispose()
        {
        }
    }
}
