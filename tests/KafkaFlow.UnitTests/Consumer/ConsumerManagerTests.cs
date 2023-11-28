using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using KafkaFlow.Configuration;
using KafkaFlow.Consumers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace KafkaFlow.UnitTests.Consumer
{
    [TestClass]
    public class ConsumerManagerTests
    {
        private readonly Fixture _fixture = new();

        private ConsumerManager _target;

        private Mock<IConsumer> _consumerMock;
        private Mock<IConsumerWorkerPool> _workerPoolMock;
        private Mock<IWorkerPoolFeeder> _feederMock;
        private Mock<ILogHandler> _logHandlerMock;
        private Mock<IDependencyResolver> _dependencyResolver;

        private Action<IDependencyResolver, Confluent.Kafka.IConsumer<byte[], byte[]>, List<Confluent.Kafka.TopicPartition>> _onPartitionAssignedHandler;
        private Action<IDependencyResolver, Confluent.Kafka.IConsumer<byte[], byte[]>, List<Confluent.Kafka.TopicPartitionOffset>> _onPartitionRevokedHandler;

        [TestInitialize]
        public void Setup()
        {
            _consumerMock = new Mock<IConsumer>();
            _workerPoolMock = new Mock<IConsumerWorkerPool>();
            _feederMock = new Mock<IWorkerPoolFeeder>();
            _logHandlerMock = new Mock<ILogHandler>();
            _dependencyResolver = new Mock<IDependencyResolver>();

            _consumerMock
                .Setup(
                    x => x.OnPartitionsAssigned(It.IsAny<Action<IDependencyResolver, Confluent.Kafka.IConsumer<byte[], byte[]>, List<Confluent.Kafka.TopicPartition>>>()))
                .Callback(
                    (Action<IDependencyResolver, Confluent.Kafka.IConsumer<byte[], byte[]>, List<Confluent.Kafka.TopicPartition>> value) =>
                        _onPartitionAssignedHandler = value);

            _consumerMock
                .Setup(
                    x => x.OnPartitionsRevoked(
                        It.IsAny<Action<IDependencyResolver, Confluent.Kafka.IConsumer<byte[], byte[]>, List<Confluent.Kafka.TopicPartitionOffset>>>()))
                .Callback(
                    (Action<IDependencyResolver, Confluent.Kafka.IConsumer<byte[], byte[]>, List<Confluent.Kafka.TopicPartitionOffset>> value) =>
                        _onPartitionRevokedHandler = value);

            var configurationMock = new Mock<IConsumerConfiguration>();

            configurationMock
                .SetupGet(x => x.WorkersCountCalculator)
                .Returns((_, _) => Task.FromResult(10));

            configurationMock
                .SetupGet(x => x.WorkersCountEvaluationInterval)
                .Returns(TimeSpan.FromMinutes(5));

            _consumerMock
                .SetupGet(x => x.Configuration)
                .Returns(configurationMock.Object);

            _target = new ConsumerManager(
                _consumerMock.Object,
                _workerPoolMock.Object,
                _feederMock.Object,
                _dependencyResolver.Object,
                _logHandlerMock.Object);
        }

        [TestMethod]
        public void ConstructorCalled_InitializeProperties()
        {
            // Assert
            _target.Consumer.Should().Be(_consumerMock.Object);
            _target.WorkerPool.Should().Be(_workerPoolMock.Object);
            _target.Feeder.Should().Be(_feederMock.Object);
        }

        [TestMethod]
        public async Task StartAsync_StartDependencies()
        {
            // Arrange
            _feederMock
                .Setup(x => x.Start());

            // Act
            await _target.StartAsync();

            // Assert
            _feederMock.VerifyAll();
        }

        [TestMethod]
        public async Task StopAsync_StopDependencies()
        {
            // Arrange
            _feederMock
                .Setup(x => x.StopAsync())
                .Returns(Task.CompletedTask);

            _workerPoolMock
                .Setup(x => x.StopAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _target.StopAsync();

            // Assert
            _feederMock.VerifyAll();
            _workerPoolMock.VerifyAll();
            _consumerMock.Verify(x => x.Dispose(), Times.Once());
        }

        [TestMethod]
        public void OnPartitionsAssigned_StartWorkerPool()
        {
            // Arrange
            var partitions = _fixture.Create<List<Confluent.Kafka.TopicPartition>>();

            _workerPoolMock
                .Setup(x => x.StartAsync(partitions, It.IsAny<int>()))
                .Returns(Task.CompletedTask);

            _logHandlerMock
                .Setup(x => x.Info(It.IsAny<string>(), It.IsAny<object>()));

            // Act
            _onPartitionAssignedHandler(_dependencyResolver.Object, Mock.Of<Confluent.Kafka.IConsumer<byte[], byte[]>>(), partitions);

            // Assert
            _workerPoolMock.VerifyAll();
            _logHandlerMock.VerifyAll();
        }

        [TestMethod]
        public void OnPartitionsRevoked_StopWorkerPool()
        {
            // Arrange
            Confluent.Kafka.IConsumer<byte[], byte[]> consumer = null;
            var partitions = _fixture.Create<List<Confluent.Kafka.TopicPartitionOffset>>();

            _workerPoolMock
                .Setup(x => x.StopAsync())
                .Returns(Task.CompletedTask);

            _consumerMock
                .SetupGet(x => x.Configuration)
                .Returns(new Mock<IConsumerConfiguration>().Object);

            _logHandlerMock
                .Setup(x => x.Warning(It.IsAny<string>(), It.IsAny<object>()));

            // Act
            _onPartitionRevokedHandler(_dependencyResolver.Object, consumer, partitions);

            // Assert
            _workerPoolMock.VerifyAll();
            _logHandlerMock.VerifyAll();
        }
    }
}
