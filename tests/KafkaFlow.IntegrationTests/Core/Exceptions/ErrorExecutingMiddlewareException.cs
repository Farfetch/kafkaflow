using System;

namespace KafkaFlow.IntegrationTests.Core.Exceptions;

public class ErrorExecutingMiddlewareException : Exception
{
    public ErrorExecutingMiddlewareException(string middlewareName)
        : base($"Exception thrown executing {middlewareName}")
    {
    }
}
