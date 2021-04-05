namespace KafkaFlow.Consumers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal class WorkerPoolFeeder : IWorkerPoolFeeder
    {
        private readonly IConsumer consumer;
        private readonly IConsumerWorkerPool workerPool;
        private readonly ILogHandler logHandler;

        private CancellationTokenSource stopTokenSource;
        private Task<Task> feederTask;

        public WorkerPoolFeeder(
            IConsumer consumer,
            IConsumerWorkerPool workerPool,
            ILogHandler logHandler)
        {
            this.consumer = consumer;
            this.workerPool = workerPool;
            this.logHandler = logHandler;
        }

        public void Start()
        {
            this.stopTokenSource = new CancellationTokenSource();

            this.feederTask = Task.Factory.StartNew(
                async () =>
                {
                    while (!this.stopTokenSource.IsCancellationRequested)
                    {
                        try
                        {
                            var message = await this.consumer
                                .ConsumeAsync(this.stopTokenSource.Token)
                                .ConfigureAwait(false);

                            await this.workerPool
                                .EnqueueAsync(message, this.stopTokenSource.Token)
                                .ConfigureAwait(false);
                        }
                        catch (OperationCanceledException)
                        {
                            // Do nothing
                        }
                        catch (Exception ex)
                        {
                            this.logHandler.Error(
                                "Error consuming message from Kafka",
                                ex,
                                null);
                        }
                    }
                },
                CancellationToken.None,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
        }

        public Task StopAsync()
        {
            if (this.stopTokenSource != null && !this.stopTokenSource.IsCancellationRequested)
            {
                this.stopTokenSource.Cancel();
                this.stopTokenSource.Dispose();
            }

            return this.feederTask ?? Task.CompletedTask;
        }
    }
}
