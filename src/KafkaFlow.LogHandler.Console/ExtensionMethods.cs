using KafkaFlow.Configuration;

namespace KafkaFlow;

/// <summary>
/// No needed
/// </summary>
public static class ExtensionMethods
{
    /// <summary>
    /// Configure KafkaFlow to use the Console to log messages
    /// </summary>
    /// <param name="builder">The Kafka configuration builder</param>
    /// <returns></returns>
    public static IKafkaConfigurationBuilder UseConsoleLog(this IKafkaConfigurationBuilder builder) =>
        builder.UseLogHandler<ConsoleLogHandler>();
}
