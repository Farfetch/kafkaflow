namespace KafkaFlow
{
    using System;

    public interface ILogHandler
    {
        void Error(string message, Exception ex, object data);

        void Info(string message, object data);

        void Warning(string message, object data);
    }
}
