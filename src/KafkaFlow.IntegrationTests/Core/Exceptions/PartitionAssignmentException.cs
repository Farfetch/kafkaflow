namespace KafkaFlow.IntegrationTests.Core.Exceptions
{
    using System;

    internal class PartitionAssignmentException : Exception
    {
        private const string ExceptionMessage = "Partition assignment hasn't occurred yet.";

        public PartitionAssignmentException()
            : base(ExceptionMessage)
        {
        }
    }
}
