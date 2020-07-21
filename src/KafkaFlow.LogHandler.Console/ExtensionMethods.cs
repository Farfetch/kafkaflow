namespace KafkaFlow
{
    using KafkaFlow.Configuration;

    /// <summary>
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Configure KafkaFlow to use the Console to log messages
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IKafkaConfigurationBuilder UseConsoleLog(this IKafkaConfigurationBuilder builder) =>
            builder.UseLogHandler<ConsoleLogHandler>();
    }
}
