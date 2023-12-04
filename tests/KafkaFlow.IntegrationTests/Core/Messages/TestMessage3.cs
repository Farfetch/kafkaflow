using System;

namespace KafkaFlow.IntegrationTests.Core.Messages;

internal class TestMessage3 : ITestMessage
{
    public Guid Id { get; set; }

    public string Value { get; set; }

    public int Version { get; set; }
}
