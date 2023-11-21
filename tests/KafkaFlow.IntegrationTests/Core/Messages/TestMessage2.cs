using System;
using System.Runtime.Serialization;

namespace KafkaFlow.IntegrationTests.Core.Messages
{
    [DataContract]
    internal class TestMessage2 : ITestMessage
    {
        [DataMember(Order = 1)]
        public Guid Id { get; set; }

        [DataMember(Order = 2)]
        public string Value { get; set; }

        [DataMember(Order = 3)]
        public int Version { get; set; }
    }
}
