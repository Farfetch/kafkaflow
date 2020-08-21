namespace KafkaFlow.Client.Messages.Adapters.Metadata
{
    using System.Linq;
    using System.Threading.Tasks;
    using KafkaFlow.Client.Messages;
    using KafkaFlow.Client.Protocol;
    using KafkaFlow.Client.Protocol.Messages;

    [ApiVersion(9)]
    internal class MetadataV9Adapter : IHostConnectionAdapter<MetadataRequest, MetadataResponse>
    {
        private readonly IKafkaHostConnection connection;

        public MetadataV9Adapter(IKafkaHostConnection connection)
        {
            this.connection = connection;
        }

        public async Task<MetadataResponse> SendAsync(MetadataRequest request)
        {
            var rawRequest = new TopicMetadataV9Request(
                request.Topics
                    .Select(x => new TopicMetadataV9Request.Topic(x))
                    .ToArray(),
                true,
                false,
                false);

            var rawResponse = await this.connection.SendAsync(rawRequest).ConfigureAwait(false);

            return new MetadataResponse(
                rawResponse.Brokers
                    .Select(x => new MetadataResponse.Broker(x.NodeId, x.Host, x.Port))
                    .ToArray(),
                rawResponse.ClusterId,
                rawResponse.ControllerId,
                rawResponse.Topics
                    .Select(
                        topic => new MetadataResponse.Topic(
                            topic.Name,
                            topic.Partitions
                                .Select(partition => new MetadataResponse.Partition(partition.Id, partition.LeaderId))
                                .ToArray())
                    )
                    .ToArray());
        }
    }
}
