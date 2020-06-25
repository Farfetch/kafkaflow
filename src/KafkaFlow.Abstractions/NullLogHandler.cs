namespace KafkaFlow
{
    using System;

    public class NullLogHandler : ILogHandler
    {
        public void Error(string message, Exception ex, object data) { }

        public void Info(string message, object data) { }

        public void Warning(string message, object data) { }
    }
}
