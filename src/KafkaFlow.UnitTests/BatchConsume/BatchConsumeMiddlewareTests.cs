namespace KafkaFlow.UnitTests.BatchConsume
{
    using System;
    using System.Threading.Tasks;
    using FluentAssertions;
    using KafkaFlow.BatchConsume;
    using KafkaFlow.Configuration;
    using KafkaFlow.Consumers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class BatchConsumeMiddlewareTests
    {
        private const int BatchSize = 3;

        private readonly TimeSpan batchTimeout = TimeSpan.FromMilliseconds(1000);
        private readonly TimeSpan waitForTaskExecution = TimeSpan.FromMilliseconds(100);

        private Mock<ILogHandler> logHandlerMock;

        private IMessageContext nextContext;
        private int timesNextWasCalled;

        private BatchConsumeMiddleware target;

        [TestInitialize]
        public void Setup()
        {
            this.nextContext = null;
            this.timesNextWasCalled = 0;

            this.logHandlerMock = new Mock<ILogHandler>();

            var middlewareContextMock = new Mock<IConsumerMiddlewareContext>();
            var workerMock = new Mock<IWorker>();
            var consumerMock = new Mock<IConsumer>();
            var consumerConfigurationMock = new Mock<IConsumerConfiguration>();

            middlewareContextMock
                .SetupGet(x => x.Worker)
                .Returns(workerMock.Object);

            middlewareContextMock
                .SetupGet(x => x.Consumer)
                .Returns(consumerMock.Object);

            consumerMock
                .SetupGet(x => x.Configuration)
                .Returns(consumerConfigurationMock.Object);

            workerMock
                .SetupGet(x => x.WorkerStopped)
                .Returns(new WorkerStoppedSubject(this.logHandlerMock.Object));

            consumerConfigurationMock
                .SetupGet(x => x.AutoMessageCompletion)
                .Returns(true);

            this.target = new BatchConsumeMiddleware(
                middlewareContextMock.Object,
                BatchSize,
                this.batchTimeout,
                this.logHandlerMock.Object);
        }

        [TestMethod]
        public async Task AddAsync_LessThanBatchSize_CallNextOnTimeout()
        {
            // Arrange
            var consumerContext = new Mock<IConsumerContext>();
            var context = new Mock<IMessageContext>();

            consumerContext
                .SetupGet(x => x.WorkerDependencyResolver)
                .Returns(Mock.Of<IDependencyResolver>());

            context
                .Setup(x => x.ConsumerContext)
                .Returns(consumerContext.Object);

            // Act
            await this.target.Invoke(context.Object, this.NextCallback);

            // Assert
            this.timesNextWasCalled.Should().Be(0);
            await this.WaitBatchTimeoutAsync();
            this.timesNextWasCalled.Should().Be(1);
            consumerContext.Verify(x => x.Complete(), Times.Once);
        }

        [TestMethod]
        public async Task AddAsync_ExactlyBatchSize_CallNextInstantly()
        {
            // Arrange
            var consumerContext = new Mock<IConsumerContext>();
            var contextMock = new Mock<IMessageContext>();

            consumerContext
                .SetupGet(x => x.WorkerDependencyResolver)
                .Returns(Mock.Of<IDependencyResolver>());

            contextMock
                .Setup(x => x.ConsumerContext)
                .Returns(consumerContext.Object);

            // Act
            for (var i = 0; i < BatchSize; i++)
            {
                await this.target.Invoke(contextMock.Object, this.NextCallback);
            }

            await Task.Delay(this.waitForTaskExecution);

            // Assert
            this.timesNextWasCalled.Should().Be(1);
            this.nextContext.GetMessagesBatch().Should().HaveCount(BatchSize);
            consumerContext.Verify(x => x.Complete(), Times.Exactly(BatchSize));
        }

        [TestMethod]
        public async Task AddAsync_MoreThanBatchSize_CallNextInstantlyThenCallWhenTimeout()
        {
            // Arrange
            var consumerContext = new Mock<IConsumerContext>();
            var contextMock = new Mock<IMessageContext>();

            consumerContext
                .SetupGet(x => x.WorkerDependencyResolver)
                .Returns(Mock.Of<IDependencyResolver>());

            contextMock
                .Setup(x => x.ConsumerContext)
                .Returns(consumerContext.Object);

            // Act
            for (var i = 0; i < BatchSize + 1; i++)
            {
                await this.target.Invoke(contextMock.Object, this.NextCallback);
            }

            await Task.Delay(this.waitForTaskExecution);

            // Assert
            this.timesNextWasCalled.Should().Be(1);
            this.nextContext.GetMessagesBatch().Should().HaveCount(BatchSize);
            consumerContext.Verify(x => x.Complete(), Times.Exactly(BatchSize));

            await this.WaitBatchTimeoutAsync();
            this.timesNextWasCalled.Should().Be(2);
            consumerContext.Verify(x => x.Complete(), Times.Exactly(BatchSize + 1));
        }

        [TestMethod]
        public async Task AddAsync_NextThrowException_LogError()
        {
            // Arrange
            var consumerContext = new Mock<IConsumerContext>();
            var contextMock = new Mock<IMessageContext>();

            contextMock
                .Setup(x => x.ConsumerContext)
                .Returns(consumerContext.Object);

            // Act
            await this.target.Invoke(contextMock.Object, _ => throw new Exception());

            // Assert
            await this.WaitBatchTimeoutAsync();
            this.logHandlerMock.Verify(x => x.Error(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<object>()), Times.Once);
        }

        private Task WaitBatchTimeoutAsync() => Task.Delay(this.batchTimeout + this.waitForTaskExecution);

        private Task NextCallback(IMessageContext ctx)
        {
            this.nextContext = ctx;
            this.timesNextWasCalled++;
            return Task.CompletedTask;
        }
    }
}
