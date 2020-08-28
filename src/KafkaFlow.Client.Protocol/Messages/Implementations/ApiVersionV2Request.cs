namespace KafkaFlow.Client.Protocol.Messages.Implementations
{
    using System;
    using System.IO;

    public class ApiVersionV2Request : IRequestMessage<ApiVersionV2Response>
    {
        public ApiKey ApiKey => ApiKey.ApiVersions;

        public short ApiVersion => 2;

        public Type ResponseType => typeof(ApiVersionV2Response);

        public void Write(Stream destination)
        {
        }
    }
}
