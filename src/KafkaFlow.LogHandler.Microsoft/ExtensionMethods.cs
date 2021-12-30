namespace KafkaFlow
{
    using KafkaFlow.Configuration;

    /// <summary>
    /// No needed
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Configure KafkaFlow to use the  Microsoft Logging framework to log messages.
        /// </summary>
        /// <param name="builder">The Kafka configuration builder</param>
        /// <returns></returns>
        public static IKafkaConfigurationBuilder UseMicrosoftLog(this IKafkaConfigurationBuilder builder) =>
            builder.UseLogHandler<MicrosoftLogHandler>();
    }
}
