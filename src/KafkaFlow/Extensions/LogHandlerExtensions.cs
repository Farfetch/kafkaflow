using Confluent.Kafka;

namespace KafkaFlow.Extensions;

internal static class LogHandlerExtensions
{
    private const string DefaultMessage = "librdkafka log";

    public static void Log(this ILogHandler logHandler, LogMessage logMessage)
    {
        var data = new
        {
            Level = logMessage.Level.ToString(),
            logMessage.Name,
            logMessage.Facility,
            logMessage.Message,
        };

        switch (logMessage.Level)
        {
            case SyslogLevel.Emergency:
            case SyslogLevel.Alert:
            case SyslogLevel.Critical:
            case SyslogLevel.Error:
                logHandler.Error(DefaultMessage, ex: null, data);
                break;
            case SyslogLevel.Warning:
                logHandler.Warning(DefaultMessage, data);
                break;
            case SyslogLevel.Notice:
            case SyslogLevel.Info:
                logHandler.Info(DefaultMessage, data);
                break;
            case SyslogLevel.Debug:
                logHandler.Verbose(DefaultMessage, data);
                break;
            default:
                logHandler.Warning(DefaultMessage, data);
                break;
        }
    }
}
