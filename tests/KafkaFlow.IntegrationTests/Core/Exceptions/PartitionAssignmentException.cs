using System;

namespace KafkaFlow.IntegrationTests.Core.Exceptions;

public class PartitionAssignmentException : Exception
{
    private const string ExceptionMessage = "Partition assignment hasn't occurred yet.";

    public PartitionAssignmentException()
        : base(ExceptionMessage)
    {
    }
}
