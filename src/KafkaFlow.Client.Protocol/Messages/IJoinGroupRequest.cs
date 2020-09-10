namespace KafkaFlow.Client.Protocol.Messages.Implementations
{
    public interface IJoinGroupRequest : IRequestMessage<IJoinGroupResponse>
    {
        string GroupId { get; set; }

        int SessionTimeoutMs { get; set; }

        int RebalanceTimeoutMs { get; set; }

        string MemberId { get; set; }

        string? GroupInstanceId { get; set; }

        string ProtocolType { get; set; }

        IProtocol[] SupportedProtocols { get; set; }

        TaggedField[] TaggedFields { get; set; }

        IProtocol CreateProtocol();

        public interface IProtocol : IRequest
        {
            string Name { get; set; }

            byte[] Metadata { get; set; }

            TaggedField[] TaggedFields { get; set; }
        }
    }
}