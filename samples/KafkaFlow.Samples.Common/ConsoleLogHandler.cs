namespace KafkaFlow.Samples.Common
{
    using System;
    using System.Text.Json;

    public class ConsoleLogHandler : ILogHandler
    {
        public void Error(string message, Exception ex, object data)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Kafka Error: {message} | Exception: {JsonSerializer.Serialize(ex)}");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void Info(string message, object data)
        {
            Console.WriteLine($"Kafka Info: {message} | Data: {JsonSerializer.Serialize(data)}");
        }

        public void Warning(string message, object data)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Kafka Warning: {message} | Data: {JsonSerializer.Serialize(data)}");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
