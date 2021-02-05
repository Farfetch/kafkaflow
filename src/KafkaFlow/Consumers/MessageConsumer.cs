namespace KafkaFlow.Consumers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Confluent.Kafka;

    internal class MessageConsumer : IMessageConsumer
    {
        private readonly IKafkaConsumer consumer;
        private readonly IConsumerWorkerPool workerPool;
        private readonly ILogHandler logHandler;

        public MessageConsumer(
            IKafkaConsumer consumer,
            IConsumerWorkerPool workerPool,
            ILogHandler logHandler)
        {
            this.workerPool = workerPool;
            this.logHandler = logHandler;
            this.consumer = consumer;
        }

        public string ConsumerName => this.consumer.Configuration.ConsumerName;

        public string GroupId => this.consumer.Configuration.GroupId;

        public int WorkerCount => this.consumer.Configuration.WorkerCount;

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
            this.consumer.Configuration.WorkerCount = workerCount;

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
            await this.consumer.StopAsync().ConfigureAwait(false);
            await Task.Delay(5000).ConfigureAwait(false);
            await this.consumer.StartAsync().ConfigureAwait(false);
        }

        public IReadOnlyList<string> Subscription => this.consumer.Subscription;

        public IReadOnlyList<TopicPartition> Assignment => this.consumer.Assignment;

        public string MemberId => this.consumer.MemberId;

        public string ClientInstanceName => this.consumer.ClientInstanceName;

        public void Pause(IEnumerable<TopicPartition> topicPartitions) =>
            this.consumer.Pause(topicPartitions);

        public void Resume(IEnumerable<TopicPartition> topicPartitions) =>
            this.consumer.Resume(topicPartitions);

        public Offset GetPosition(TopicPartition topicPartition) =>
            this.consumer.GetPosition(topicPartition);

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
