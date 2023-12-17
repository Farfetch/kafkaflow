using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using SolTechnology.Avro.Infrastructure.Attributes;

namespace KafkaFlow.IntegrationTests.Core.Messages;

[DataContract(Name = "TestAvroConvertMessage", Namespace = "KafkaFlow.IntegrationTests.Core.Messages")]
public class TestAvroConvertMessage : ITestMessage
{
    public Guid Id { get; set; }

    public string Value { get; set; }

    public int Version { get; set; }

    [DataMember(Name = "DifferentValue")]
    [Description("Changing the name of the property")]
    [NullableSchema]
    public string AnotherValue { get; set; }
}
