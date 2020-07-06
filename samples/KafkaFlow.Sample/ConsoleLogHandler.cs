namespace KafkaFlow.Sample
{
    using System;
    using System.Text.Json;

    public class ConsoleLogHandler : ILogHandler
    {
        public void Error(string message, Exception ex, object data)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\nKafka Error: {message} | Exception: {JsonSerializer.Serialize(ex)}");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void Info(string message, object data)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nKafka Info: {message} | Data: {JsonSerializer.Serialize(data)}");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void Warning(string message, object data)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\nKafka Warning: {message} | Data: {JsonSerializer.Serialize(data)}");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
