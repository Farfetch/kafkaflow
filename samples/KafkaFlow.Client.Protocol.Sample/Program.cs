namespace KafkaFlow.Client.Sample
{
    using System;
    using System.Linq;
    using System.Text.Json;
    using System.Threading.Tasks;
    using KafkaFlow.Client.Protocol;
    using KafkaFlow.Client.Protocol.Messages;
    using KafkaFlow.Client.Protocol.Messages.Implementations.Metadata;
    using KafkaFlow.Client.Protocol.Messages.Implementations.OffsetFetch;

    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting...");

            const string groupId = "print-console-handler";
            const string topicName = "test-topic";
            string broker = "localhost";
            var options = new JsonSerializerOptions() { };
            var connection = new BrokerConnection(
                new BrokerAddress(broker, 9092),
                "test-client-id",
                TimeSpan.FromTicks(10001));

            var topicMetadata = await connection.SendAsync(
                new MetadataV9Request
                {
                    Topics = new IMetadataRequest.ITopic[]
                    {
                        new MetadataV9Request.Topic { Name = topicName }
                    }
                });
            
            Console.WriteLine($"{DateTime.Now:O} - TOPIC METADATA: {JsonSerializer.Serialize(topicMetadata, options)}.");

            var partitions = topicMetadata
                .Topics
                .First(t => t.Name == topicName)
                .Partitions
                .Select(p => p.Id)
                .ToArray();

            var committedOffsets = await connection.SendAsync(new OffsetFetchV5Request(groupId, topicName, partitions ));
            Console.WriteLine($"{DateTime.Now:O} - OFFSET FROM CONSUMER GROUP: {JsonSerializer.Serialize(topicMetadata, options)}.");
            
            var lastOffsets = await connection.SendAsync(new ListOffsetsV5Request(-1, 0, topicName, partitions));
            Console.WriteLine($"{DateTime.Now:O} - OFFSET FROM TOPIC: {JsonSerializer.Serialize(lastOffsets, options)}");

            Console.WriteLine("Ended.");
        }
    }
}
