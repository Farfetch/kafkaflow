namespace KafkaFlow.Admin.WebApi.Contracts
{
    using System.Collections.Generic;
    using KafkaFlow.Consumers;

    public class GroupResponse
    {
        public string GroupId { get; set; }
        public IEnumerable<IMessageConsumer> Consumers { get; set; }
    }
}
