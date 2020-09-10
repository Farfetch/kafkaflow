namespace KafkaFlow.Client.Protocol.Messages.Implementations
{
    using System;
    using System.IO;

    public class ApiVersionV2Request : IApiVersionRequest
    {
        public ApiKey ApiKey => ApiKey.ApiVersions;

        public short ApiVersion => 2;

        public Type ResponseType => typeof(ApiVersionV2Response);

        public void Write(Stream destination)
        {
            // Do Nothing
        }
    }
}
