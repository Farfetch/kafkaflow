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
        public async Task StopAsync_BlockedOnConsume_MustStop()
        {
            // Arrange
            var consumeResult = new ConsumeResult<byte[], byte[]>();

            this.consumerMock
                .Setup(x => x.ConsumeAsync(It.IsAny<CancellationToken>()))
                .Returns(
                    new ValueTask<ConsumeResult<byte[], byte[]>>(
                        Task.Delay(Timeout.Infinite).ContinueWith(_ => consumeResult)));

            // Act
            this.target.Start();
            await Task.Delay(100);
            await this.target.StopAsync();

            // Assert
            this.consumerMock.VerifyAll();
        }

        [TestMethod]
        [Timeout(1000)]
        public async Task StopAsync_BlockedOnQueuing_MustStop()
        {
            // Arrange
            var consumeResult = new ConsumeResult<byte[], byte[]>();

            this.consumerMock
                .Setup(x => x.ConsumeAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(consumeResult);

            this.workerPoolMock
                .Setup(x => x.EnqueueAsync(consumeResult, It.IsAny<CancellationToken>()))
                .Returns(Task.Delay(Timeout.Infinite));

            // Act
            this.target.Start();
            await Task.Delay(100);
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

            this.consumerMock
                .SetupSequence(x => x.ConsumeAsync(It.IsAny<CancellationToken>()))
                .Throws(exception)
                .ReturnsAsync(consumeResult);

            this.workerPoolMock
                .Setup(x => x.EnqueueAsync(consumeResult, It.IsAny<CancellationToken>()))
                .Returns(Task.Delay(Timeout.Infinite));

            this.logHandlerMock
                .Setup(x => x.Error(It.IsAny<string>(), exception, It.IsAny<object>()));

            // Act
            this.target.Start();
            await Task.Delay(100);
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

            this.consumerMock
                .Setup(x => x.ConsumeAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(consumeResult);

            this.workerPoolMock
                .SetupSequence(x => x.EnqueueAsync(consumeResult, It.IsAny<CancellationToken>()))
                .Throws(exception)
                .Returns(Task.Delay(Timeout.Infinite));

            this.logHandlerMock
                .Setup(x => x.Error(It.IsAny<string>(), exception, It.IsAny<object>()));

            // Act
            this.target.Start();
            await Task.Delay(100);
            await this.target.StopAsync();

            // Assert
            this.consumerMock.VerifyAll();
            this.workerPoolMock.VerifyAll();
            this.logHandlerMock.VerifyAll();
        }
    }
}
