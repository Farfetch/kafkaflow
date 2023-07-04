namespace KafkaFlow.Consumers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Confluent.Kafka;

    internal class MessageConsumer : IMessageConsumer
    {
        private static readonly IReadOnlyList<TopicPartition> EmptyTopicPartition = new List<TopicPartition>().AsReadOnly();

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

        public string ClusterName => this.consumerManager.Consumer.Configuration.ClusterConfiguration.Name;

        public bool ManagementDisabled => this.consumerManager.Consumer.Configuration.ManagementDisabled;

        public string GroupId => this.consumerManager.Consumer.Configuration.GroupId;

        public IReadOnlyList<string> Topics => this.consumerManager.Consumer.Configuration.Topics;

        public IReadOnlyList<string> Subscription => this.consumerManager.Consumer.Subscription;

        public IReadOnlyList<TopicPartition> Assignment => this.consumerManager.Consumer.Assignment ?? EmptyTopicPartition;

        public ConsumerStatus Status => this.consumerManager.Consumer.Status;

        public string MemberId => this.consumerManager.Consumer.MemberId;

        public string ClientInstanceName => this.consumerManager.Consumer.ClientInstanceName;

        public int WorkersCount => this.consumerManager.WorkerPool.CurrentWorkersCount;

        public IReadOnlyList<TopicPartition> PausedPartitions =>
            this.consumerManager.Consumer.FlowManager?.PausedPartitions ??
            EmptyTopicPartition;

        public IEnumerable<TopicPartition> RunningPartitions => this.Assignment.Except(this.PausedPartitions);

        public async Task StartAsync()
        {
            await this.consumerManager.StartAsync().ConfigureAwait(false);
            this.logHandler.Info($"Kafka consumer '{this.ConsumerName}' was manually started", null);
        }

        public async Task StopAsync()
        {
            await this.consumerManager.StopAsync().ConfigureAwait(false);
            this.logHandler.Info($"Kafka consumer '{this.ConsumerName}' was manually stopped", null);
        }

        public async Task RestartAsync()
        {
            await this.InternalRestart().ConfigureAwait(false);
            this.logHandler.Info($"Kafka consumer '{this.ConsumerName}' was manually restarted", null);
        }

        public void Pause(IReadOnlyCollection<TopicPartition> topicPartitions)
        {
            this.consumerManager.Consumer.FlowManager.Pause(topicPartitions);
            this.logHandler.Info($"Kafka consumer '{this.ConsumerName}' was paused", topicPartitions);
        }

        public void Resume(IReadOnlyCollection<TopicPartition> topicPartitions)
        {
            this.consumerManager.Consumer.FlowManager.Resume(topicPartitions);
            this.logHandler.Info($"Kafka consumer '{this.ConsumerName}' was resumed", topicPartitions);
        }

        public Offset GetPosition(TopicPartition topicPartition) =>
            this.consumerManager.Consumer.GetPosition(topicPartition);

        public WatermarkOffsets GetWatermarkOffsets(TopicPartition topicPartition) =>
            this.consumerManager.Consumer.GetWatermarkOffsets(topicPartition);

        public WatermarkOffsets QueryWatermarkOffsets(TopicPartition topicPartition, TimeSpan timeout) =>
            this.consumerManager.Consumer.QueryWatermarkOffsets(topicPartition, timeout);

        public List<TopicPartitionOffset> GetOffsets(
            IEnumerable<TopicPartitionTimestamp> topicPartitions,
            TimeSpan timeout) =>
            this.consumerManager.Consumer.OffsetsForTimes(topicPartitions, timeout);

        public IEnumerable<TopicPartitionLag> GetTopicPartitionsLag() =>
            this.consumerManager.Consumer.GetTopicPartitionsLag();

        public async Task OverrideOffsetsAndRestartAsync(IReadOnlyCollection<TopicPartitionOffset> offsets)
        {
            try
            {
                await this.consumerManager.Feeder.StopAsync().ConfigureAwait(false);
                await this.consumerManager.WorkerPool.StopAsync().ConfigureAwait(false);

                this.consumerManager.Consumer.Commit(offsets);

                await this.InternalRestart().ConfigureAwait(false);

                this.logHandler.Info($"Offsets of Kafka consumer '{this.ConsumerName}' were overridden ", GetOffsetsLogData(offsets));
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

        public async Task ChangeWorkersCountAndRestartAsync(int workersCount)
        {
            this.consumerManager.Consumer.Configuration.WorkersCountCalculator = _ => Task.FromResult(workersCount);

            await this.InternalRestart().ConfigureAwait(false);

            this.logHandler.Info(
                $"Total of workers in KafkaFlow consumer '{this.ConsumerName}' were updated",
                new { workersCount });
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
