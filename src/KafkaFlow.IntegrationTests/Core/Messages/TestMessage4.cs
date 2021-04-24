namespace KafkaFlow.IntegrationTests.Core.Messages
{
    using System;

    internal class TestMessage4 : ITestMessage
    {
        public Guid Id { get; set; }

        public string Value { get; set; }

        public string OtherValue { get; set; }

        public int Version { get; set; }
    }
}
