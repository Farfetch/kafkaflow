namespace KafkaFlow.IntegrationTests.Core.Messages
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public class PauseResumeMessage : ITestMessage
    {
        [DataMember(Order = 1)]
        public Guid Id { get; set; }

        [DataMember(Order = 2)]
        public string Value { get; set; }
        
        [DataMember(Order = 3)]
        public int Version { get; set; }
    }
}
