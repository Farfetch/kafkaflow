namespace KafkaFlow
{
    using System;
    using System.Text.Json;

    internal class ConsoleLogHandler : ILogHandler
    {
        public void Error(string message, Exception ex, object data) => Print(
            $"KafkaFlow: {message} | Data: {JsonSerializer.Serialize(data)} | Exception: {JsonSerializer.Serialize(new {ex.Message, ex.Source, ex.HelpLink, ex.HResult, ex.StackTrace})}",
            ConsoleColor.Red);

        public void Info(string message, object data) => Print(
            $"KafkaFlow: {message} | Data: {JsonSerializer.Serialize(data)}",
            ConsoleColor.Green);

        public void Warning(string message, object data) => Print(
            $"KafkaFlow: {message} | Data: {JsonSerializer.Serialize(data)}",
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
