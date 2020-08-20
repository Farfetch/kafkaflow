namespace KafkaFlow.Admin.Messages
{
    using System.Runtime.Serialization;

    [DataContract]
    public class ChangeConsumerWorkerCount : IAdminMessage
    {
        [DataMember(Order = 1)]
        public string ConsumerName { get; set; }

        [DataMember(Order = 2)]
        public int WorkerCount { get; set; }
    }
}
