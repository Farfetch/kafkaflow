namespace KafkaFlow.Client.Protocol
{
    using System;
    using KafkaFlow.Client.Protocol.Messages;
    using KafkaFlow.Client.Protocol.Streams;

    internal class Request : IRequest
    {
        public Request(
            int correlationId,
            string clientId,
            IRequestMessage message)
        {
            this.CorrelationId = correlationId;
            this.ClientId = clientId;
            this.Message = message;
        }

        private int CorrelationId { get; }

        private string ClientId { get; }

        private TaggedField[] TaggedFields => Array.Empty<TaggedField>();

        private IRequestMessage Message { get; }

        void IRequest.Write(MemoryWriter destination)
        {
            // Skip message size to write later
            destination.WriteInt32(0);

            var startPosition = destination.Position;
            destination.WriteInt16((short)this.Message.ApiKey);
            destination.WriteInt16(this.Message.ApiVersion);
            destination.WriteInt32(this.CorrelationId);
            destination.WriteString(this.ClientId);

            if (this.Message is ITaggedFields)
            {
                destination.WriteTaggedFields(this.TaggedFields);
            }

            destination.WriteMessage(this.Message);

            var endPosition = destination.Position;

            // Write message size at the beginning
            destination.Position = 0;
            destination.WriteInt32(Convert.ToInt32(endPosition - startPosition));
        }
    }
}
