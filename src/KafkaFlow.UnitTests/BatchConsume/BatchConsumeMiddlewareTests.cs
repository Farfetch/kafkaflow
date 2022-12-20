namespace KafkaFlow.UnitTests.BatchConsume
{
    using System;
    using System.Threading.Tasks;
    using FluentAssertions;
    using KafkaFlow.BatchConsume;
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

            this.target = new BatchConsumeMiddleware(
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

            context
                .Setup(x => x.ConsumerContext)
                .Returns(consumerContext.Object);

            // Act
            await this.target.Invoke(context.Object, this.NextCallback);

            // Assert
            this.timesNextWasCalled.Should().Be(0);
            await this.WaitBatchTimeoutAsync();
            this.timesNextWasCalled.Should().Be(1);
            consumerContext.Verify(x => x.StoreOffset(), Times.Once);
        }

        [TestMethod]
        public async Task AddAsync_ExactlyBatchSize_CallNextInstantly()
        {
            // Arrange
            var consumerContext = new Mock<IConsumerContext>();
            var contextMock = new Mock<IMessageContext>();

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
            consumerContext.Verify(x => x.StoreOffset(), Times.Exactly(BatchSize));
        }

        [TestMethod]
        public async Task AddAsync_MoreThanBatchSize_CallNextInstantlyThenCallWhenTimeout()
        {
            // Arrange
            var consumerContext = new Mock<IConsumerContext>();
            var contextMock = new Mock<IMessageContext>();

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
            consumerContext.Verify(x => x.StoreOffset(), Times.Exactly(BatchSize));

            await this.WaitBatchTimeoutAsync();
            this.timesNextWasCalled.Should().Be(2);
            consumerContext.Verify(x => x.StoreOffset(), Times.Exactly(BatchSize + 1));
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
