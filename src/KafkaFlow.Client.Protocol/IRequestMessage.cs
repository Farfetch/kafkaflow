namespace KafkaFlow.Client.Protocol
{
    using System;

    public interface IRequestMessage<TResponse> : IRequestMessage where TResponse : IResponse
    {
        public Type ResponseType { get; }
    }

    public interface IRequestMessageV2<TResponse> : IRequestMessage<TResponse>, IRequestV2 where TResponse : IResponseV2
    {
    }

    public interface IRequestMessage : IRequest
    {
        public ApiKey ApiKey { get; }

        public short ApiVersion { get; }
    }
}
