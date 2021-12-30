namespace KafkaFlow
{
    using System;
    using System.Text.Json;
    using Microsoft.Extensions.Logging;

    internal class MicrosoftLogHandler : ILogHandler
    {
        private readonly ILogger logger;

        public MicrosoftLogHandler(ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger("KafkaFlow");
        }

        public void Error(string message, Exception ex, object data)
        {
            this.logger.LogError(ex, "{Message} | Data: {Data}", message, JsonSerializer.Serialize(data));
        }

        public void Warning(string message, object data)
        {
            this.logger.LogWarning("{Message} | Data: {Data}", message, JsonSerializer.Serialize(data));
        }

        public void Info(string message, object data)
        {
            this.logger.LogInformation("{Message} | Data: {Data}", message, JsonSerializer.Serialize(data));
        }

        public void Verbose(string message, object data)
        {
            this.logger.LogDebug("{Message} | Data: {Data}", message, JsonSerializer.Serialize(data));
        }
    }
}
