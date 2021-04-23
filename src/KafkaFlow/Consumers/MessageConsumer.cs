namespace KafkaFlow.Consumers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Confluent.Kafka;

    internal class MessageConsumer : IMessageConsumer
    {
        private readonly IConsumerManager consumerManager;
        private readonly ILogHandler logHandler;

        public MessageConsumer(
            IConsumerManager consumerManager,
            ILogHandler logHandler)
        {
            this.consumerManager = consumerManager;
            this.logHandler = logHandler;
        }

        public string ConsumerName => this.consumerManager.Consumer.Configuration.ConsumerName;

        public string GroupId => this.consumerManager.Consumer.Configuration.GroupId;

        public IReadOnlyList<string> Subscription => this.consumerManager.Consumer.Subscription;

        public IReadOnlyList<TopicPartition> Assignment => this.consumerManager.Consumer.Assignment;

        public string MemberId => this.consumerManager.Consumer.MemberId;

        public string ClientInstanceName => this.consumerManager.Consumer.ClientInstanceName;

        public int WorkerCount => this.consumerManager.Consumer.Configuration.WorkerCount;

        public void Pause(IReadOnlyCollection<TopicPartition> topicPartitions) =>
            this.consumerManager.Consumer.FlowManager.Pause(topicPartitions);

        public void Resume(IReadOnlyCollection<TopicPartition> topicPartitions) =>
            this.consumerManager.Consumer.FlowManager.Resume(topicPartitions);

        public Offset GetPosition(TopicPartition topicPartition) =>
            this.consumerManager.Consumer.GetPosition(topicPartition);

        public WatermarkOffsets GetWatermarkOffsets(TopicPartition topicPartition) =>
            this.consumerManager.Consumer.GetWatermarkOffsets(topicPartition);

        public WatermarkOffsets QueryWatermarkOffsets(TopicPartition topicPartition, TimeSpan timeout) =>
            this.consumerManager.Consumer.QueryWatermarkOffsets(topicPartition, timeout);

        public List<TopicPartitionOffset> OffsetsForTimes(
            IEnumerable<TopicPartitionTimestamp> topicPartitions,
            TimeSpan timeout) =>
            this.consumerManager.Consumer.OffsetsForTimes(topicPartitions, timeout);

        public async Task OverrideOffsetsAndRestartAsync(IReadOnlyCollection<TopicPartitionOffset> offsets)
        {
            if (offsets is null)
            {
                throw new ArgumentNullException(nameof(offsets));
            }

            try
            {
                await this.consumerManager.Feeder.StopAsync().ConfigureAwait(false);
                await this.consumerManager.WorkerPool.StopAsync().ConfigureAwait(false);

                this.consumerManager.Consumer.Commit(offsets);

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

        public async Task ChangeWorkerCountAndRestartAsync(int workerCount)
        {
            this.consumerManager.Consumer.Configuration.WorkerCount = workerCount;

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
                            Offset = y.Offset.Value,
                        }),
                });

        private async Task InternalRestart()
        {
            await this.consumerManager.StopAsync().ConfigureAwait(false);
            await Task.Delay(5000).ConfigureAwait(false);
            await this.consumerManager.StartAsync().ConfigureAwait(false);
        }
    }
}
