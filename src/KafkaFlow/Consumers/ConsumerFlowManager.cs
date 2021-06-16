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
        private readonly IConsumer consumer;
        private readonly IConsumer<byte[], byte[]> clientConsumer;
        private readonly ILogHandler logHandler;
        private readonly List<TopicPartition> pausedPartitions = new();

        private CancellationTokenSource heartbeatTokenSource;
        private Task heartbeatTask;

        public ConsumerFlowManager(
            IConsumer consumer,
            IConsumer<byte[], byte[]> clientConsumer,
            ILogHandler logHandler)
        {
            this.consumer = consumer;
            this.clientConsumer = clientConsumer;
            this.logHandler = logHandler;
        }

        public IReadOnlyList<TopicPartition> PausedPartitions => this.pausedPartitions.AsReadOnly();

        public void Pause(IReadOnlyCollection<TopicPartition> topicPartitions)
        {
            lock (this.pausedPartitions)
            {
                topicPartitions = topicPartitions.Except(this.pausedPartitions).ToList();

                if (!topicPartitions.Any())
                {
                    return;
                }

                this.clientConsumer.Pause(topicPartitions);
                this.pausedPartitions.AddRange(topicPartitions);

                if (this.consumer.Status != ConsumerStatus.Paused)
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
                                var result = this.clientConsumer.Consume(consumeTimeoutCall);

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

                if (this.consumer.Status == ConsumerStatus.Paused)
                {
                    return;
                }

                this.heartbeatTokenSource?.Cancel();

                this.heartbeatTask.GetAwaiter().GetResult();
                this.heartbeatTask.Dispose();

                this.clientConsumer.Resume(topicPartitions);
            }
        }

        public void CleanPausedPartitions()
        {
            this.pausedPartitions.Clear();
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
