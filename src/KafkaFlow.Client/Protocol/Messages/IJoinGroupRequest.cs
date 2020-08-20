namespace KafkaFlow.Client.Protocol.Messages
{
    public interface IJoinGroupRequest : IRequestMessage<IJoinGroupResponse>
    {
        string GroupId { get; }

        int SessionTimeoutMs { get; }

        int RebalanceTimeoutMs { get; }

        string MemberId { get; }

        string? GroupInstanceId { get; }

        string ProtocolType { get; }

        IProtocol[] SupportedProtocols { get; }

        TaggedField[] TaggedFields { get; }

        IProtocol CreateProtocol();

        public interface IProtocol : IRequest
        {
            string Name { get; }

            byte[] Metadata { get; }

            TaggedField[] TaggedFields { get; }
        }
    }
}
