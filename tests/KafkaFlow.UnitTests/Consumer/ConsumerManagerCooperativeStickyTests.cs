using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Confluent.Kafka;
using FluentAssertions;
using KafkaFlow.Configuration;
using KafkaFlow.Consumers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace KafkaFlow.UnitTests.Consumer;

[TestClass]
public class ConsumerManagerCooperativeStickyTests
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
            .Setup(x => x.GetKafkaConfig())
            .Returns(() => new ConsumerConfig { PartitionAssignmentStrategy = PartitionAssignmentStrategy.CooperativeSticky });

        configurationMock
            .SetupGet(x => x.WorkersCountCalculator)
            .Returns((_, _) => Task.FromResult(10));

        configurationMock
            .SetupGet(x => x.WorkersCountEvaluationInterval)
            .Returns(TimeSpan.FromMinutes(5));

        _consumerMock
            .SetupGet(x => x.Configuration)
            .Returns(configurationMock.Object);

        _consumerMock
            .SetupGet(x => x.Assignment)
            .Returns([]);

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
        _workerPoolMock.VerifyNoOtherCalls();
        _consumerMock.Verify(x => x.Dispose(), Times.Once());
    }

    [TestMethod]
    public void OnPartitionsAssigned_StartWorkerPool()
    {
        // Arrange
        var currentPartitions = _fixture.Create<List<Confluent.Kafka.TopicPartition>>();
        var newAssignedPartitions = _fixture.Create<List<Confluent.Kafka.TopicPartition>>();
        var allPartitions = currentPartitions.Concat(newAssignedPartitions).ToArray();

        _workerPoolMock
            .Setup(x => x.StopAsync())
            .Returns(Task.CompletedTask);

        _workerPoolMock
            .Setup(x => x.StartAsync(allPartitions, It.IsAny<int>()))
            .Returns(Task.CompletedTask);

        _logHandlerMock
            .Setup(x => x.Info(It.IsAny<string>(), It.IsAny<object>()));

        _consumerMock.SetupGet(x => x.Assignment).Returns(allPartitions.ToArray());

        // Act
        _onPartitionAssignedHandler(_dependencyResolver.Object, Mock.Of<Confluent.Kafka.IConsumer<byte[], byte[]>>(), newAssignedPartitions);

        // Assert
        _workerPoolMock.VerifyAll();
        _workerPoolMock.VerifyNoOtherCalls();
        _logHandlerMock.VerifyAll();
    }

    [TestMethod]
    public void OnPartitionsRevoked_StopWorkerPool()
    {
        // Arrange
        var currentPartitions = _fixture.CreateMany<Confluent.Kafka.TopicPartition>(6).ToList();
        var revokedPartitions = currentPartitions.Take(3).ToArray();
        var leftPartitions = currentPartitions.Except(revokedPartitions).ToArray();

        _workerPoolMock
            .Setup(x => x.StopAsync())
            .Returns(Task.CompletedTask);

        _workerPoolMock
            .Setup(x => x.StartAsync(leftPartitions, It.IsAny<int>()))
            .Returns(Task.CompletedTask);

        _consumerMock
            .SetupGet(x => x.Configuration)
            .Returns(new Mock<IConsumerConfiguration>().Object);

        _logHandlerMock
            .Setup(x => x.Warning(It.IsAny<string>(), It.IsAny<object>()));

        _consumerMock.SetupGet(x => x.Assignment).Returns(leftPartitions);

        // Act
        _onPartitionRevokedHandler(_dependencyResolver.Object, Mock.Of<Confluent.Kafka.IConsumer<byte[], byte[]>>(), revokedPartitions.Select(x => new Confluent.Kafka.TopicPartitionOffset(x, 123)).ToList());

        // Assert
        _workerPoolMock.VerifyAll();
        _workerPoolMock.VerifyNoOtherCalls();
        _logHandlerMock.VerifyAll();
    }
}
