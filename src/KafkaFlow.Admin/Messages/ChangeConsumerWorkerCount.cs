namespace KafkaFlow.Admin.Messages
{
    using System.Runtime.Serialization;

    /// <summary>
    /// A message to change the worker count
    /// </summary>
    [DataContract]
    public class ChangeConsumerWorkerCount : IAdminMessage
    {
        /// <summary>
        /// Gets or sets the consumer name that will be affected
        /// </summary>
        [DataMember(Order = 1)]
        public string ConsumerName { get; set; }

        /// <summary>
        /// Gets or sets the number of workers
        /// </summary>
        [DataMember(Order = 2)]
        public int WorkerCount { get; set; }
    }
}
