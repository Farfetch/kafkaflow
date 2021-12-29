namespace KafkaFlow.Client.Protocol.Messages.Implementations.ApiVersion
{
    using System;
    using KafkaFlow.Client.Protocol.Streams;

    internal class ApiVersionV2Request : IApiVersionRequest
    {
        public ApiKey ApiKey => ApiKey.ApiVersions;

        public short ApiVersion => 2;

        public Type ResponseType => typeof(ApiVersionV2Response);

        void IRequest.Write(MemoryWriter destination)
        {
            // Do Nothing
        }
    }
}
