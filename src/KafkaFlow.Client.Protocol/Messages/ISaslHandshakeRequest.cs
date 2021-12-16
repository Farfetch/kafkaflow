namespace KafkaFlow.Client.Protocol.Messages
{
    public interface ISaslHandshakeRequest : IRequestMessage<ISaslHandshakeResponse>
    {
        string Mechanism { get; }
    }
}
