namespace KafkaFlow.Client.Protocol
{
    using System;

    public interface IRequestMessage<TResponse> : IRequestMessage where TResponse : IResponse
    {
        public Type ResponseType { get; }
    }

    public interface IRequestMessage : IRequest
    {
        public ApiKey ApiKey { get; }

        public short ApiVersion { get; }
    }
}
