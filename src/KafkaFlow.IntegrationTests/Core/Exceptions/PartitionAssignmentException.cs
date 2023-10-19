namespace KafkaFlow.IntegrationTests.Core.Exceptions
{
    using System;

    public class PartitionAssignmentException : Exception
    {
        private const string ExceptionMessage = "Partition assignment hasn't occurred yet.";

        public PartitionAssignmentException()
            : base(ExceptionMessage)
        {
        }
    }
}
