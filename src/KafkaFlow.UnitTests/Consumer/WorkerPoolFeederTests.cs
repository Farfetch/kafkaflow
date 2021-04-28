namespace KafkaFlow.UnitTests.Consumer
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using KafkaFlow.Consumers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class WorkerPoolFeederTests
    {
        private WorkerPoolFeeder target;

        private Mock<IConsumer> consumerMock;
        private Mock<IConsumerWorkerPool> workerPoolMock;
        private Mock<ILogHandler> logHandlerMock;

        [TestInitialize]
        public void Setup()
        {
            this.consumerMock = new Mock<IConsumer>(MockBehavior.Strict);
            this.workerPoolMock = new Mock<IConsumerWorkerPool>(MockBehavior.Strict);
            this.logHandlerMock = new Mock<ILogHandler>(MockBehavior.Strict);

            this.target = new WorkerPoolFeeder(
                this.consumerMock.Object,
                this.workerPoolMock.Object,
                this.logHandlerMock.Object);
        }

        [TestMethod]
        [Timeout(1000)]
        public async Task StopAsync_WithoutStarting_Return()
        {
            // Act
            await this.target.StopAsync();
        }

        [TestMethod]
        [Timeout(1000)]
        public async Task StopAsync_WaitingOnConsumeWithCancellation_MustStop()
        {
            // Arrange
            var ready = new ManualResetEvent(false);

            this.consumerMock
                .Setup(x => x.ConsumeAsync(It.IsAny<CancellationToken>()))
                .Returns(
                    async (CancellationToken ct) =>
                    {
                        ready.Set();
                        await Task.Delay(Timeout.Infinite, ct);
                        return default; // Never reached
                    });

            // Act
            this.target.Start();
            ready.WaitOne();
            await this.target.StopAsync();

            // Assert
            this.consumerMock.Verify(x => x.ConsumeAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        [Timeout(1000)]
        public async Task StopAsync_WaitingOnQueuingWithCancellation_MustStop()
        {
            // Arrange
            var consumeResult = new ConsumeResult<byte[], byte[]>();
            var ready = new ManualResetEvent(false);

            this.consumerMock
                .Setup(x => x.ConsumeAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(consumeResult);

            this.workerPoolMock
                .Setup(x => x.EnqueueAsync(consumeResult, It.IsAny<CancellationToken>()))
                .Returns((ConsumeResult<byte[], byte[]> _, CancellationToken ct) =>
                {
                    ready.Set();
                    return Task.Delay(Timeout.Infinite, ct);
                });

            // Act
            this.target.Start();
            ready.WaitOne();
            await this.target.StopAsync();

            // Assert
            this.consumerMock.VerifyAll();
            this.workerPoolMock.VerifyAll();
        }

        [TestMethod]
        [Timeout(1000)]
        public async Task ConsumeAsyncThrows_LogAndCallConsumeAsyncAgain()
        {
            // Arrange
            var consumeResult = new ConsumeResult<byte[], byte[]>();
            var exception = new Exception();
            var ready = new ManualResetEvent(false);

            this.consumerMock
                .SetupSequence(x => x.ConsumeAsync(It.IsAny<CancellationToken>()))
                .Throws(exception)
                .ReturnsAsync(consumeResult);

            this.workerPoolMock
                .Setup(x => x.EnqueueAsync(consumeResult, It.IsAny<CancellationToken>()))
                .Returns((ConsumeResult<byte[], byte[]> _, CancellationToken ct) =>
                {
                    ready.Set();
                    return Task.Delay(Timeout.Infinite, ct);
                });

            this.logHandlerMock
                .Setup(x => x.Error(It.IsAny<string>(), exception, It.IsAny<object>()));

            // Act
            this.target.Start();
            ready.WaitOne();
            await this.target.StopAsync();

            // Assert
            this.consumerMock.VerifyAll();
            this.workerPoolMock.VerifyAll();
            this.logHandlerMock.VerifyAll();
        }

        [TestMethod]
        [Timeout(1000)]
        public async Task EnqueueAsyncThrows_LogAndCallConsumeAsyncAgain()
        {
            // Arrange
            var consumeResult = new ConsumeResult<byte[], byte[]>();
            var exception = new Exception();
            var ready = new ManualResetEvent(false);

            this.consumerMock
                .Setup(x => x.ConsumeAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(consumeResult);

            var hasThrown = false;
            this.workerPoolMock
                .Setup(x => x.EnqueueAsync(consumeResult, It.IsAny<CancellationToken>()))
                .Returns((ConsumeResult<byte[], byte[]> _, CancellationToken ct) =>
                {
                    ready.Set();

                    if (!hasThrown)
                    {
                        hasThrown = true;
                        throw exception;
                    }

                    return Task.Delay(Timeout.Infinite, ct);
                });

            this.logHandlerMock
                .Setup(x => x.Error(It.IsAny<string>(), exception, It.IsAny<object>()));

            // Act
            this.target.Start();
            ready.WaitOne();
            await this.target.StopAsync();

            // Assert
            this.consumerMock.VerifyAll();
            this.workerPoolMock.VerifyAll();
            this.logHandlerMock.VerifyAll();
        }
    }
}
