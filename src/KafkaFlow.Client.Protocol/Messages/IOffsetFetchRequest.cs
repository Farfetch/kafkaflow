namespace KafkaFlow.Client.Protocol.Messages
{
    public interface IOffsetFetchRequest : IRequestMessage<IOffsetFetchResponse>
    {
        string GroupId { get; }

        ITopic[] Topics { get; }

        public interface ITopic : IRequest
        {
            string Name { get; }

            int[] Partitions { get; }
        }
    }
}
