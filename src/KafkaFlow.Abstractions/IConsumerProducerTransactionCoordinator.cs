namespace KafkaFlow
{
    /// <summary>
    /// Coordinates consumer/producer transaction execution
    /// </summary>
    public interface IConsumerProducerTransactionCoordinator
    {
        /// <summary>
        /// Signal initiated transaction
        /// </summary>
        void Initiated();

        /// <summary>
        /// Signal completed transaction
        /// </summary>
        void Completed();
    }
}
