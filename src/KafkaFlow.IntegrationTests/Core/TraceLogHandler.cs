namespace KafkaFlow.IntegrationTests.Core
{
    using System;
    using System.Diagnostics;
    using System.Text.Json;

    public class TraceLogHandler : ILogHandler
    {
        public void Error(string message, Exception ex, object data)
        {
            Trace.TraceError(
                JsonSerializer.Serialize(
                    new
                    {
                        Message = message,
                        Exception = ex,
                        Data = data
                    }));
        }

        public void Info(string message, object data)
        {
            Trace.TraceInformation(
                JsonSerializer.Serialize(
                    new
                    {
                        Message = message,
                        Data = data
                    }));
        }

        public void Warning(string message, object data)
        {
            Trace.TraceWarning(
                JsonSerializer.Serialize(
                    new
                    {
                        Message = message,
                        Data = data
                    }));
        }
    }
}
