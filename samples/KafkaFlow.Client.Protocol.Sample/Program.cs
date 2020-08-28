namespace KafkaFlow.Client.Sample
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using KafkaFlow.Client.Protocol;
    using KafkaFlow.Client.Protocol.Messages;

    class Program
    {
        static async Task Main(string[] args)
        {
            // const string groupId = "print-console-handler";
            //
            // var connection = new KafkaHostConnection(
            //     "localhost",
            //     9092,
            //     "test-client-id",
            //     TimeSpan.FromSeconds(30));
            //
            // var apiVersion = await connection.SendAsync(
            //     new ApiVersionV2Request());
            //
            // var topicMetadata = await connection.SendAsync(
            //     new MetadataV9Request(
            //         new[] { new MetadataV9Request.Topic("test-topic") },
            //         false,
            //         true,
            //         true));
            //
            // var findCoordResponse = await connection.SendAsync(
            //     new FindCoordinatorV3Request(string.Empty, 0));
            //
            // var joinGroupResponse = await connection.SendAsync(
            //     new JoinGroupV7Request(
            //         "print-console-handler",
            //         300000,
            //         3000,
            //         string.Empty,
            //         null,
            //         "consumer",
            //         new[] { new JoinGroupV7Request.Protocol("consumer", Array.Empty<byte>()), }));
            //
            // var joinGroupResponse1 = await connection.SendAsync(
            //     new JoinGroupV7Request(
            //         "print-console-handler",
            //         300000,
            //         3000,
            //         joinGroupResponse.MemberId,
            //         null,
            //         "consumer",
            //         new[] { new JoinGroupV7Request.Protocol("consumer", Array.Empty<byte>()), }));
            //
            //
            // var heartbeatResponse = await connection.SendAsync(
            //     new HeartbeatV4Request(
            //         groupId,
            //         joinGroupResponse1.GenerationId,
            //         joinGroupResponse1.MemberId));
            //
            // var offsetFetchResponse = await connection.SendAsync(
            //     new OffsetFetchV5Request(
            //         groupId,
            //         new[]
            //         {
            //             new OffsetFetchV5Request.Topic(
            //                 "test-topic",
            //                 new[] { 0, 1, 2 })
            //         }));
            //
            // // var produceResponse = await ProduceMessage(connection);
            // var fetchResponse = await FetchMessage(connection);

            await Task.Delay(5000);
        }
        //
        // private static Task<FetchV11Response> FetchMessage(KafkaHostConnection connection)
        // {
        //     return connection.SendAsync(
        //         new FetchV11Request
        //         {
        //             ReplicaId = -1,
        //             MaxWaitTime = 5000,
        //             MinBytes = 0,
        //             MaxBytes = 1024 * 16 * 3,
        //             IsolationLevel = 1,
        //             Topics = new[]
        //             {
        //                 new FetchV11Request.Topic
        //                 {
        //                     Name = "test-topic",
        //                     Partitions = new[]
        //                     {
        //                         new FetchV11Request.Partition
        //                         {
        //                             Id = 0,
        //                             FetchOffset = 0,
        //                             PartitionMaxBytes = 1024 * 16
        //                         },
        //                         new FetchV11Request.Partition
        //                         {
        //                             Id = 1,
        //                             FetchOffset = 0,
        //                             PartitionMaxBytes = 1024 * 16
        //                         },
        //                         new FetchV11Request.Partition
        //                         {
        //                             Id = 2,
        //                             FetchOffset = 0,
        //                             PartitionMaxBytes = 1024 * 16
        //                         },
        //                     }
        //                 }
        //             }
        //         });
        // }

        // private static Task<IProduceResponse> ProduceMessage(IKafkaHostConnection connection)
        // {
        //     var batch = new RecordBatch();
        //
        //     var topic = new ProduceV8Request.Topic("test-client");
        //
        //     topic.Partitions.TryAdd(
        //         0,
        //         new ProduceV8Request.Partition(0)
        //         {
        //             RecordBatch = batch
        //         });
        //
        //     batch.AddRecord(
        //         new RecordBatch.Record
        //         {
        //             Key = Encoding.UTF8.GetBytes("teste_key"),
        //             Value = Encoding.UTF8.GetBytes("teste_value"),
        //             Headers = new[]
        //             {
        //                 new RecordBatch.Header
        //                 {
        //                     Key = "teste_header_key",
        //                     Value = Encoding.UTF8.GetBytes("teste_header_value")
        //                 }
        //             }
        //         });
        //
        //     var request = new ProduceV8Request(ProduceAcks.Leader, 5000);
        //
        //     request.Topics.TryAdd(topic.Name, topic);
        //
        //     return connection.SendAsync(request);
        // }
    }
}
