using System;

namespace KafkaFlow.IntegrationTests.Core.Messages;

internal interface ITestMessage
{
    Guid Id { get; set; }

    string Value { get; set; }

    int Version { get; set; }
}
