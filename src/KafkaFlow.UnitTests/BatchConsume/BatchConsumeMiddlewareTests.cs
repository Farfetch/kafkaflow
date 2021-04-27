namespace KafkaFlow.UnitTests.BatchConsume
{
    using System;
    using System.Threading.Tasks;
    using KafkaFlow.BatchConsume;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    internal class BatchConsumeMiddlewareTests
    {
        private const int BatchSize = 10;
        private readonly TimeSpan batchTimeout = TimeSpan.FromSeconds(3);

        private Mock<IWorkerBatchFactory> workerBatchFactoryMock;
        private Mock<ILogHandler> logHandlerMock;

        private BatchConsumeMiddleware target;

        [TestInitialize]
        public void Setup()
        {
            this.workerBatchFactoryMock = new Mock<IWorkerBatchFactory>(MockBehavior.Strict);
            this.logHandlerMock = new Mock<ILogHandler>(MockBehavior.Strict);

            this.target = new BatchConsumeMiddleware(
                BatchSize,
                this.batchTimeout,
                this.workerBatchFactoryMock.Object,
                this.logHandlerMock.Object);
        }

        [TestMethod]
        public async Task Invoke_DifferentWorkers_CallAddAsyncForEachOne()
        {
            // Arrange
            var contextWorker1Mock = new Mock<IMessageContext>();
            var contextWorker2Mock = new Mock<IMessageContext>();

            var consumerContextWorker1Mock = new Mock<IConsumerContext>();
            var consumerContextWorker2Mock = new Mock<IConsumerContext>();

            var worker1Batch = new Mock<IWorkerBatch>();
            var worker2Batch = new Mock<IWorkerBatch>();

            var nextMock = new Mock<MiddlewareDelegate>();

            consumerContextWorker1Mock
                .SetupGet(x => x.WorkerId)
                .Returns(1);

            contextWorker1Mock
                .SetupGet(x => x.ConsumerContext)
                .Returns(consumerContextWorker1Mock.Object);

            consumerContextWorker1Mock.SetupSet(x => x.ShouldStoreOffset = false);

            consumerContextWorker2Mock
                .SetupGet(x => x.WorkerId)
                .Returns(2);

            contextWorker2Mock
                .SetupGet(x => x.ConsumerContext)
                .Returns(consumerContextWorker2Mock.Object);

            consumerContextWorker2Mock.SetupSet(x => x.ShouldStoreOffset = false);

            worker1Batch
                .Setup(x => x.AddAsync(contextWorker1Mock.Object, nextMock.Object))
                .Returns(Task.CompletedTask);

            worker2Batch
                .Setup(x => x.AddAsync(contextWorker2Mock.Object, nextMock.Object))
                .Returns(Task.CompletedTask);

            this.workerBatchFactoryMock
                .SetupSequence(x => x.Create(BatchSize, this.batchTimeout, this.logHandlerMock.Object))
                .Returns(worker1Batch.Object)
                .Returns(worker2Batch.Object);

            // Act
            await this.target.Invoke(contextWorker1Mock.Object, nextMock.Object);
            await this.target.Invoke(contextWorker2Mock.Object, nextMock.Object);

            // Assert
            this.workerBatchFactoryMock.VerifyAll();
            worker1Batch.VerifyAll();
            worker2Batch.VerifyAll();
            contextWorker1Mock.VerifyAll();
            contextWorker2Mock.VerifyAll();
            consumerContextWorker1Mock.VerifyAll();
            consumerContextWorker2Mock.VerifyAll();
        }
    }
}
