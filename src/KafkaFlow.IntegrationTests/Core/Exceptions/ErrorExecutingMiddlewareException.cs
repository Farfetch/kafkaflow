namespace KafkaFlow.IntegrationTests.Core.Exceptions
{
    using System;

    public class ErrorExecutingMiddlewareException : Exception
    {
        public ErrorExecutingMiddlewareException(string middlewareName)
            : base($"Exception thrown executing {middlewareName}")
        {
        }
    }
}
