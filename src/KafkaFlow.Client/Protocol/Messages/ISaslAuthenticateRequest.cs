namespace KafkaFlow.Client.Protocol.Messages
{
    public interface ISaslAuthenticateRequest : IRequestMessage<ISaslAuthenticateResponse>
    {
        byte[] AuthBytes { get; }
    }
}
