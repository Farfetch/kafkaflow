namespace KafkaFlow.Client.Protocol.Messages
{
    public interface ISaslAuthenticateResponse : IResponse
    {
        short ErrorCode { get; }

        string ErrorMessage { get; }

        byte[] AuthBytes { get; }

        long SessionLifetimeMs { get; }
    }
}
