using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KafkaFlow.Consumers
{
    internal class MessageConsumer : IMessageConsumer
    {
        private static readonly IReadOnlyList<Confluent.Kafka.TopicPartition> s_emptyTopicPartition = new List<Confluent.Kafka.TopicPartition>().AsReadOnly();

        private readonly IConsumerManager _consumerManager;
        private readonly ILogHandler _logHandler;

        public MessageConsumer(
            IConsumerManager consumerManager,
            ILogHandler logHandler)
        {
            _consumerManager = consumerManager;
            _logHandler = logHandler;
        }

        public string ConsumerName => _consumerManager.Consumer.Configuration.ConsumerName;

        public string ClusterName => _consumerManager.Consumer.Configuration.ClusterConfiguration.Name;

        public bool ManagementDisabled => _consumerManager.Consumer.Configuration.ManagementDisabled;

        public string GroupId => _consumerManager.Consumer.Configuration.GroupId;

        public IReadOnlyList<string> Topics => _consumerManager.Consumer.Configuration.Topics;

        public IReadOnlyList<string> Subscription => _consumerManager.Consumer.Subscription;

        public IReadOnlyList<Confluent.Kafka.TopicPartition> Assignment => _consumerManager.Consumer.Assignment ?? s_emptyTopicPartition;

        public ConsumerStatus Status => _consumerManager.Consumer.Status;

        public string MemberId => _consumerManager.Consumer.MemberId;

        public string ClientInstanceName => _consumerManager.Consumer.ClientInstanceName;

        public int WorkersCount => _consumerManager.WorkerPool.CurrentWorkersCount;

        public IReadOnlyList<Confluent.Kafka.TopicPartition> PausedPartitions =>
            _consumerManager.Consumer.FlowManager?.PausedPartitions ??
            s_emptyTopicPartition;

        public IEnumerable<Confluent.Kafka.TopicPartition> RunningPartitions => this.Assignment.Except(this.PausedPartitions);

        public async Task StartAsync()
        {
            await _consumerManager.StartAsync().ConfigureAwait(false);
            _logHandler.Info($"Kafka consumer '{this.ConsumerName}' was manually started", null);
        }

        public async Task StopAsync()
        {
            await _consumerManager.StopAsync().ConfigureAwait(false);
            _logHandler.Info($"Kafka consumer '{this.ConsumerName}' was manually stopped", null);
        }

        public async Task RestartAsync()
        {
            await this.InternalRestart().ConfigureAwait(false);
            _logHandler.Info($"Kafka consumer '{this.ConsumerName}' was manually restarted", null);
        }

        public void Pause(IReadOnlyCollection<Confluent.Kafka.TopicPartition> topicPartitions)
        {
            _consumerManager.Consumer.FlowManager.Pause(topicPartitions);
            _logHandler.Info($"Kafka consumer '{this.ConsumerName}' was paused", topicPartitions);
        }

        public void Resume(IReadOnlyCollection<Confluent.Kafka.TopicPartition> topicPartitions)
        {
            _consumerManager.Consumer.FlowManager.Resume(topicPartitions);
            _logHandler.Info($"Kafka consumer '{this.ConsumerName}' was resumed", topicPartitions);
        }

        public Confluent.Kafka.Offset GetPosition(Confluent.Kafka.TopicPartition topicPartition) =>
            _consumerManager.Consumer.GetPosition(topicPartition);

        public Confluent.Kafka.WatermarkOffsets GetWatermarkOffsets(Confluent.Kafka.TopicPartition topicPartition) =>
            _consumerManager.Consumer.GetWatermarkOffsets(topicPartition);

        public Confluent.Kafka.WatermarkOffsets QueryWatermarkOffsets(Confluent.Kafka.TopicPartition topicPartition, TimeSpan timeout) =>
            _consumerManager.Consumer.QueryWatermarkOffsets(topicPartition, timeout);

        public List<Confluent.Kafka.TopicPartitionOffset> GetOffsets(
            IEnumerable<Confluent.Kafka.TopicPartitionTimestamp> topicPartitions,
            TimeSpan timeout) =>
            _consumerManager.Consumer.OffsetsForTimes(topicPartitions, timeout);

        public IEnumerable<TopicPartitionLag> GetTopicPartitionsLag() =>
            _consumerManager.Consumer.GetTopicPartitionsLag();

        public async Task OverrideOffsetsAndRestartAsync(IReadOnlyCollection<Confluent.Kafka.TopicPartitionOffset> offsets)
        {
            try
            {
                await _consumerManager.Feeder.StopAsync().ConfigureAwait(false);
                await _consumerManager.WorkerPool.StopAsync().ConfigureAwait(false);

                _consumerManager.Consumer.Commit(offsets);

                await this.InternalRestart().ConfigureAwait(false);

                _logHandler.Info($"Offsets of Kafka consumer '{this.ConsumerName}' were overridden ", GetOffsetsLogData(offsets));
            }
            catch (Exception e)
            {
                _logHandler.Error(
                    "Error overriding offsets",
                    e,
                    GetOffsetsLogData(offsets));
                throw;
            }
        }

        public async Task ChangeWorkersCountAndRestartAsync(int workersCount)
        {
            _consumerManager.Consumer.Configuration.WorkersCountCalculator = (_, _) => Task.FromResult(workersCount);

            await this.InternalRestart().ConfigureAwait(false);

            _logHandler.Info(
                $"Total of workers in KafkaFlow consumer '{this.ConsumerName}' were updated",
                new { workersCount });
        }

        private static object GetOffsetsLogData(IEnumerable<Confluent.Kafka.TopicPartitionOffset> offsets) => offsets
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
            await _consumerManager.StopAsync().ConfigureAwait(false);
            await Task.Delay(5000).ConfigureAwait(false);
            await _consumerManager.StartAsync().ConfigureAwait(false);
        }
    }
}
