namespace KafkaFlow.Client.Protocol.Messages
{
    public interface ISaslHandshakeResponse : IResponse
    {
        short ErrorCode { get; }

        string[] Mechanisms { get; }
    }
}
