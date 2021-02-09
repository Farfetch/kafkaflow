namespace KafkaFlow.Consumers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;

    internal class KafkaConsumerFlowManager : IKafkaConsumerFlowManager
    {
        private readonly IConsumer<byte[], byte[]> consumer;
        private readonly CancellationToken stopCancellationToken;
        private readonly ILogHandler logHandler;

        private CancellationTokenSource heartbeatTokenSource;
        private Task heartbeatTask;

        private readonly List<TopicPartition> pausedPartitions = new List<TopicPartition>();

        public KafkaConsumerFlowManager(
            IConsumer<byte[], byte[]> consumer,
            CancellationToken stopCancellationToken,
            ILogHandler logHandler)
        {
            this.consumer = consumer;
            this.stopCancellationToken = stopCancellationToken;
            this.logHandler = logHandler;
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

                if (this.HasRunningPartitions())
                {
                    return;
                }

                this.heartbeatTokenSource = CancellationTokenSource.CreateLinkedTokenSource(this.stopCancellationToken);

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

                if (!this.HasRunningPartitions())
                {
                    return;
                }

                this.heartbeatTokenSource?.Cancel();

                this.heartbeatTask.GetAwaiter().GetResult();
                this.heartbeatTask.Dispose();

                this.consumer.Resume(topicPartitions);
            }
        }

        private bool HasRunningPartitions()
        {
            return
                this.consumer.Assignment.Count != this.pausedPartitions.Count ||
                this.consumer.Assignment.Except(this.pausedPartitions).Any();
        }
    }
}
