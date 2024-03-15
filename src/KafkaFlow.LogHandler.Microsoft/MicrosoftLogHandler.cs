using System;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace KafkaFlow;

internal class MicrosoftLogHandler : ILogHandler
{
    private const string SerializationErrorMessage = "Log data serialization error.";

    private readonly ILogger _logger;

    public MicrosoftLogHandler(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger("KafkaFlow");
    }

    public void Error(string message, Exception ex, object data)
    {
        _logger.LogError(ex, "{Message} | Data: {Data}", message, SerializeData(data));
    }

    public void Warning(string message, object data)
    {
        _logger.LogWarning("{Message} | Data: {Data}", message, SerializeData(data));
    }

    public void Warning(string message, Exception ex, object data)
    {
        _logger.LogWarning(ex, "{Message} | Data: {Data}", message, SerializeData(data));
    }

    public void Info(string message, object data)
    {
        _logger.LogInformation("{Message} | Data: {Data}", message, SerializeData(data));
    }

    public void Verbose(string message, object data)
    {
        _logger.LogDebug("{Message} | Data: {Data}", message, SerializeData(data));
    }

    private string SerializeData(object data)
    {
        if (data is null)
        {
            return null;
        }

        try
        {
            return JsonSerializer.Serialize(data);
        }
        catch(Exception ex)
        {
            return JsonSerializer.Serialize(new
            {
                Error = SerializationErrorMessage,
                Exception = $"Message: {ex.Message}, Trace: {ex.StackTrace}",
            });
        }
    }
}
