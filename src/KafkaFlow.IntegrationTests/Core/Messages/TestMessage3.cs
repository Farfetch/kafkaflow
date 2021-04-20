namespace KafkaFlow.IntegrationTests.Core.Messages
{
    using System;

    public class TestMessage3 : ITestMessage
    {
        public Guid Id { get; set; }
        public string Value { get; set; }
        public int Version { get; set; }
    }
}