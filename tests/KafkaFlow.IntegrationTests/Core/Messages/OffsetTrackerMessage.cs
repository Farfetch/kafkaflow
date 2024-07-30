using System;

namespace KafkaFlow.IntegrationTests.Core.Messages;

internal class OffsetTrackerMessage
{
    public Guid Id { get; set; }
    public long Offset { get; set; }
}