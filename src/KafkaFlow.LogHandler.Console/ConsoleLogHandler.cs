namespace KafkaFlow
{
    using System;
    using System.Text.Json;

    internal class ConsoleLogHandler : ILogHandler
    {
        public void Error(string message, Exception ex, object data)
        {
            var serializedException = JsonSerializer.Serialize(
                new
                {
                    Type = ex.GetType().FullName,
                    ex.Message,
                    ex.StackTrace
                });

            Print(
                $"\nKafkaFlow: {message} | Data: {JsonSerializer.Serialize(data)} | Exception: {serializedException}",
                ConsoleColor.Red);
        }

        public void Info(string message, object data) => Print(
            $"\nKafkaFlow: {message} | Data: {JsonSerializer.Serialize(data)}",
            ConsoleColor.Green);

        public void Warning(string message, object data) => Print(
            $"\nKafkaFlow: {message} | Data: {JsonSerializer.Serialize(data)}",
            ConsoleColor.Yellow);

        private static void Print(string message, ConsoleColor color)
        {
            var colorBefore = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = colorBefore;
        }
    }
}
