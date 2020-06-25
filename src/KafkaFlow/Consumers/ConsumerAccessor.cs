namespace KafkaFlow.Consumers
{
    /// <summary>
    /// Provides access to configured consumers
    /// </summary>
    public static class ConsumerAccessor
    {
        /// <summary>
        /// Gets the default instance of <see cref="ConsumerAccessor"/>
        /// </summary>
        public static IConsumerAccessor Instance => ConsumerManager.Instance;
    }
}
