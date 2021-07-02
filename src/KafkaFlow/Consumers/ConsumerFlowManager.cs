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
        private readonly ILogHandler logHandler;
        private readonly List<TopicPartition> pausedPartitions = new();
        private readonly SemaphoreSlim consumerSemaphore = new(1, 1);

        private IConsumer<byte[], byte[]> clientConsumer;
        private CancellationTokenSource heartbeatTokenSource;
        private Task heartbeatTask;

        public ConsumerFlowManager(
            IConsumer consumer,
            ILogHandler logHandler)
        {
            this.consumer = consumer;
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

                this.StartHeartbeat();
            }
        }

        public Task BlockHeartbeat(CancellationToken cancellationToken)
        {
            return this.consumerSemaphore.WaitAsync(cancellationToken);
        }

        public void ReleaseHeartbeat()
        {
            if (this.consumerSemaphore.CurrentCount != 1)
            {
                this.consumerSemaphore.Release();
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

                this.StopHeartbeat();

                this.clientConsumer.Resume(topicPartitions);
            }
        }

        public void Start(IConsumer<byte[], byte[]> clientConsumer)
        {
            this.clientConsumer = clientConsumer;
        }

        public void Stop()
        {
            this.pausedPartitions.Clear();
            this.StopHeartbeat();
        }

        private void StartHeartbeat()
        {
            this.heartbeatTokenSource = new CancellationTokenSource();

            this.heartbeatTask = Task.Run(
                () =>
                {
                    if (this.consumerSemaphore.Wait(0))
                    {
                        try
                        {
                            const int consumeTimeoutCall = 1000;

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
                        finally
                        {
                            this.ReleaseHeartbeat();
                        }
                    }
                });
        }

        private void StopHeartbeat()
        {
            this.heartbeatTokenSource?.Cancel();
            this.heartbeatTask?.GetAwaiter().GetResult();
            this.heartbeatTask?.Dispose();
        }
    }
}
