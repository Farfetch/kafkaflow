namespace KafkaFlow.Client.Protocol.Messages.Implementations
{
    public interface IApiVersionResponse : IResponse
    {
        ErrorCode Error { get; }

        IApiVersion[] ApiVersions { get; }

        int ThrottleTime { get; }

        public interface IApiVersion : IResponse
        {
            ApiKey ApiKey { get; }

            short MinVersion { get; }

            short MaxVersion { get; }
        }
    }
}
