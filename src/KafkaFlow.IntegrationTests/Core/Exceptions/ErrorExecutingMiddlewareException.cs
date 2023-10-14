namespace KafkaFlow.IntegrationTests.Core.Exceptions
{
    using System;

    internal class ErrorExecutingMiddlewareException : Exception
    {
        public ErrorExecutingMiddlewareException(string middlewareName)
            : base($"Exception thrown executing {middlewareName}")
        {
        }
    }
}
