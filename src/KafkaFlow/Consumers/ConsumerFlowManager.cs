namespace KafkaFlow.Consumers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;

    internal class ConsumerFlowManager : IConsumerFlowManager
    {
        private readonly IConsumer<byte[], byte[]> consumer;
        private readonly ILogHandler logHandler;
        private readonly List<TopicPartition> pausedPartitions = new();

        private CancellationTokenSource heartbeatTokenSource;
        private Task heartbeatTask;

        public ConsumerFlowManager(
            IConsumer<byte[], byte[]> consumer,
            ILogHandler logHandler)
        {
            this.consumer = consumer;
            this.logHandler = logHandler;
        }

        public IReadOnlyList<TopicPartition> PausedPartitions => this.pausedPartitions.AsReadOnly();

        public ConsumerFlowStatus Status
        {
            get
            {
                if (this.pausedPartitions.Count == 0)
                {
                    return ConsumerFlowStatus.Running;
                }

                return this.pausedPartitions.Count == this.consumer.Assignment.Count ?
                    ConsumerFlowStatus.Paused :
                    ConsumerFlowStatus.PartiallyRunning;
            }
        }

        public void Pause(IReadOnlyCollection<TopicPartition> topicPartitions)
        {
            lock (this.pausedPartitions)
            {
                topicPartitions = topicPartitions.Except(this.pausedPartitions).ToList();

                if (!topicPartitions.Any())
                {
                    return;
                }

                this.consumer.Pause(topicPartitions);
                this.pausedPartitions.AddRange(topicPartitions);

                if (this.Status != ConsumerFlowStatus.Paused)
                {
                    return;
                }

                this.heartbeatTokenSource = new CancellationTokenSource();

                this.heartbeatTask = Task.Run(
                    () =>
                    {
                        const int consumeTimeoutCall = 1000;

                        try
                        {
                            while (!this.heartbeatTokenSource.IsCancellationRequested)
                            {
                                var result = this.consumer.Consume(consumeTimeoutCall);

                                if (result != null)
                                {
                                    this.logHandler.Warning(
                                        "Paused consumer heartbeat process wrongly read a message, please report this issue",
                                        null);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            this.logHandler.Error(
                                "Error executing paused consumer background heartbeat",
                                ex,
                                null);
                        }
                    },
                    this.heartbeatTokenSource.Token);
            }
        }

        public void Resume(IReadOnlyCollection<TopicPartition> topicPartitions)
        {
            lock (this.pausedPartitions)
            {
                if (!topicPartitions.Any())
                {
                    return;
                }

                foreach (var topicPartition in topicPartitions)
                {
                    this.pausedPartitions.Remove(topicPartition);
                }

                if (this.Status == ConsumerFlowStatus.Paused)
                {
                    return;
                }

                this.heartbeatTokenSource?.Cancel();

                this.heartbeatTask.GetAwaiter().GetResult();
                this.heartbeatTask.Dispose();

                this.consumer.Resume(topicPartitions);
            }
        }

        public void UpdatePausedPartitions(IEnumerable<TopicPartition> partitionsRunning)
        {
            foreach (var p in partitionsRunning)
            {
                this.pausedPartitions.Remove(p);
            }
        }

        public void Dispose()
        {
            this.heartbeatTokenSource?.Cancel();
            this.heartbeatTokenSource?.Dispose();

            this.heartbeatTask?.GetAwaiter().GetResult();
            this.heartbeatTask?.Dispose();
        }
    }
}
