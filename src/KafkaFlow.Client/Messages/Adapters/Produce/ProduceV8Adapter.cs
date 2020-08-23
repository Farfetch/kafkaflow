namespace KafkaFlow.Client.Messages.Adapters.Produce
{
    using System.Linq;
    using System.Threading.Tasks;
    using KafkaFlow.Client.Messages;
    using KafkaFlow.Client.Protocol;
    using KafkaFlow.Client.Protocol.Messages;

    [ApiVersion(8)]
    internal class ProduceV8Adapter : IHostConnectionAdapter<ProduceRequest, ProduceResponse>
    {
        private readonly IKafkaHostConnection connection;

        public ProduceV8Adapter(IKafkaHostConnection connection)
        {
            this.connection = connection;
        }

        public async Task<ProduceResponse> SendAsync(ProduceRequest request)
        {
            var rawRequest = new ProduceV8Request(
                request.Acks,
                request.Timeout,
                request.Topics
                    .Select(
                        topic => new ProduceV8Request.Topic(
                            topic.Value.Name,
                            topic.Value.Partitions
                                .Select(
                                    partition => new ProduceV8Request.Partition(
                                        partition.Value.Id,
                                        partition.Value.Batch))
                                .ToArray())
                    )
                    .ToArray());

            var rawResponse = await this.connection.SendAsync(rawRequest).ConfigureAwait(false);

            return new ProduceResponse(
                rawResponse.Topics
                    .Select(
                        topic => new ProduceResponse.Topic(
                            topic.Name,
                            topic.Partitions
                                .Select(
                                    partition => new ProduceResponse.Partition(
                                        partition.Id,
                                        partition.Error,
                                        partition.BaseOffset,
                                        partition.Errors
                                            .Select(
                                                error => new ProduceResponse.RecordError(
                                                    error.BatchIndex,
                                                    error.Message))
                                            .ToArray(),
                                        partition.ErrorMessage))
                                .ToArray())
                    )
                    .ToArray());
        }
    }
}
