namespace KafkaFlow.Consumers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using KafkaFlow.Configuration;

    internal class MessageConsumer : IMessageConsumer
    {
        private readonly IConsumer<byte[], byte[]> consumer;
        private readonly KafkaConsumer kafkaConsumer;
        private readonly IConsumerWorkerPool workerPool;
        private readonly ConsumerConfiguration configuration;
        private readonly ILogHandler logHandler;

        public MessageConsumer(
            IConsumer<byte[], byte[]> consumer,
            KafkaConsumer kafkaConsumer,
            IConsumerWorkerPool workerPool,
            ConsumerConfiguration configuration,
            ILogHandler logHandler)
        {
            this.workerPool = workerPool;
            this.configuration = configuration;
            this.logHandler = logHandler;
            this.consumer = consumer;
            this.kafkaConsumer = kafkaConsumer;
        }

        public string ConsumerName => this.configuration.ConsumerName;

        public string GroupId => this.configuration.GroupId;

        public async Task OverrideOffsetsAndRestartAsync(IReadOnlyCollection<TopicPartitionOffset> offsets)
        {
            if (offsets is null)
            {
                throw new ArgumentNullException(nameof(offsets));
            }

            try
            {
                this.consumer.Pause(this.consumer.Assignment);
                await this.workerPool.StopAsync().ConfigureAwait(false);

                this.consumer.Commit(offsets);

                await this.InternalRestart().ConfigureAwait(false);

                this.logHandler.Info("Kafka offsets overridden", GetOffsetsLogData(offsets));
            }
            catch (Exception e)
            {
                this.logHandler.Error(
                    "Error overriding offsets",
                    e,
                    GetOffsetsLogData(offsets));
                throw;
            }
        }

        private static object GetOffsetsLogData(IEnumerable<TopicPartitionOffset> offsets) => offsets
            .GroupBy(x => x.Topic)
            .Select(
                x => new
                {
                    x.First().Topic,
                    Partitions = x.Select(
                        y => new
                        {
                            Partition = y.Partition.Value,
                            Offset = y.Offset.Value
                        })
                });

        public async Task ChangeWorkerCountAndRestartAsync(int workerCount)
        {
            this.configuration.WorkerCount = workerCount;
            await this.InternalRestart().ConfigureAwait(false);

            this.logHandler.Info(
                "KafkaFlow consumer workers changed",
                new { workerCount });
        }

        public async Task RestartAsync()
        {
            await this.InternalRestart().ConfigureAwait(false);
            this.logHandler.Info("KafkaFlow consumer manually restarted", null);
        }

        private async Task InternalRestart()
        {
            await this.kafkaConsumer.StopAsync().ConfigureAwait(false);
            await Task.Delay(5000).ConfigureAwait(false);
            await this.kafkaConsumer.StartAsync().ConfigureAwait(false);
        }

        public IReadOnlyList<string> Subscription => this.consumer.Subscription;

        public IReadOnlyList<TopicPartition> Assignment => this.consumer.Assignment;

        public string MemberId => this.consumer.MemberId;

        public string ClientInstanceName => this.consumer.Name;

        public void Pause(IEnumerable<TopicPartition> topicPartitions) =>
            this.consumer.Pause(topicPartitions);

        public void Resume(IEnumerable<TopicPartition> topicPartitions) =>
            this.consumer.Resume(topicPartitions);

        public Offset GetPosition(TopicPartition topicPartition) =>
            this.consumer.Position(topicPartition);

        public WatermarkOffsets GetWatermarkOffsets(TopicPartition topicPartition) =>
            this.consumer.GetWatermarkOffsets(topicPartition);

        public WatermarkOffsets QueryWatermarkOffsets(TopicPartition topicPartition, TimeSpan timeout) =>
            this.consumer.QueryWatermarkOffsets(topicPartition, timeout);

        public List<TopicPartitionOffset> OffsetsForTimes(
            IEnumerable<TopicPartitionTimestamp> topicPartitions,
            TimeSpan timeout) =>
            this.consumer.OffsetsForTimes(topicPartitions, timeout);
    }
}
