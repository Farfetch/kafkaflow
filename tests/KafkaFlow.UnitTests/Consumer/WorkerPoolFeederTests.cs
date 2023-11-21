using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using KafkaFlow.Consumers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace KafkaFlow.UnitTests.Consumer
{
    [TestClass]
    public class WorkerPoolFeederTests
    {
        private WorkerPoolFeeder _target;

        private Mock<IConsumer> _consumerMock;
        private Mock<IConsumerWorkerPool> _workerPoolMock;
        private Mock<ILogHandler> _logHandlerMock;

        [TestInitialize]
        public void Setup()
        {
            _consumerMock = new Mock<IConsumer>(MockBehavior.Strict);
            _workerPoolMock = new Mock<IConsumerWorkerPool>(MockBehavior.Strict);
            _logHandlerMock = new Mock<ILogHandler>(MockBehavior.Strict);

            _target = new WorkerPoolFeeder(
                _consumerMock.Object,
                _workerPoolMock.Object,
                _logHandlerMock.Object);
        }

        [TestMethod]
        [Timeout(1000)]
        public async Task StopAsync_WithoutStarting_Return()
        {
            // Act
            await _target.StopAsync();
        }

        [TestMethod]
        [Timeout(1000)]
        public async Task StopAsync_WaitingOnConsumeWithCancellation_MustStop()
        {
            // Arrange
            var ready = new ManualResetEvent(false);

            _consumerMock
                .Setup(x => x.ConsumeAsync(It.IsAny<CancellationToken>()))
                .Returns(
                    async (CancellationToken ct) =>
                    {
                        ready.Set();
                        await Task.Delay(Timeout.Infinite, ct);
                        return default; // Never reached
                    });

            // Act
            _target.Start();
            ready.WaitOne();
            await _target.StopAsync();

            // Assert
            _consumerMock.Verify(x => x.ConsumeAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        [Timeout(1000)]
        public async Task StopAsync_WaitingOnQueuingWithCancellation_MustStop()
        {
            // Arrange
            var consumeResult = new ConsumeResult<byte[], byte[]>();
            var ready = new ManualResetEvent(false);

            _consumerMock
                .Setup(x => x.ConsumeAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(consumeResult);

            _workerPoolMock
                .Setup(x => x.EnqueueAsync(consumeResult, It.IsAny<CancellationToken>()))
                .Returns((ConsumeResult<byte[], byte[]> _, CancellationToken ct) =>
                {
                    ready.Set();
                    return Task.Delay(Timeout.Infinite, ct);
                });

            // Act
            _target.Start();
            ready.WaitOne();
            await _target.StopAsync();

            // Assert
            _consumerMock.VerifyAll();
            _workerPoolMock.VerifyAll();
        }

        [TestMethod]
        [Timeout(1000)]
        public async Task ConsumeAsyncThrows_LogAndCallConsumeAsyncAgain()
        {
            // Arrange
            var consumeResult = new ConsumeResult<byte[], byte[]>();
            var exception = new Exception();
            var ready = new ManualResetEvent(false);

            _consumerMock
                .SetupSequence(x => x.ConsumeAsync(It.IsAny<CancellationToken>()))
                .Throws(exception)
                .ReturnsAsync(consumeResult);

            _workerPoolMock
                .Setup(x => x.EnqueueAsync(consumeResult, It.IsAny<CancellationToken>()))
                .Returns((ConsumeResult<byte[], byte[]> _, CancellationToken ct) =>
                {
                    ready.Set();
                    return Task.Delay(Timeout.Infinite, ct);
                });

            _logHandlerMock
                .Setup(x => x.Error(It.IsAny<string>(), exception, It.IsAny<object>()));

            // Act
            _target.Start();
            ready.WaitOne();
            await _target.StopAsync();

            // Assert
            _consumerMock.VerifyAll();
            _workerPoolMock.VerifyAll();
            _logHandlerMock.VerifyAll();
        }

        [TestMethod]
        [Timeout(1000)]
        public async Task EnqueueAsyncThrows_LogAndCallConsumeAsyncAgain()
        {
            // Arrange
            var consumeResult = new ConsumeResult<byte[], byte[]>();
            var exception = new Exception();
            var ready = new ManualResetEvent(false);

            _consumerMock
                .Setup(x => x.ConsumeAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(consumeResult);

            var hasThrown = false;
            _workerPoolMock
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

            _logHandlerMock
                .Setup(x => x.Error(It.IsAny<string>(), exception, It.IsAny<object>()));

            // Act
            _target.Start();
            ready.WaitOne();
            await _target.StopAsync();

            // Assert
            _consumerMock.VerifyAll();
            _workerPoolMock.VerifyAll();
            _logHandlerMock.VerifyAll();
        }
    }
}
