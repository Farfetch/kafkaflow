using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Confluent.Kafka;
using AwesomeAssertions;
using KafkaFlow.Configuration;
using KafkaFlow.Consumers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace KafkaFlow.UnitTests.Consumer;

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

        var configurationMock= new Mock<IConsumerConfiguration>();

        configurationMock
            .Setup(x => x.GetKafkaConfig())
            .Returns(() => new ConsumerConfig { PartitionAssignmentStrategy = PartitionAssignmentStrategy.RoundRobin });

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
        var partitions = _fixture.Create<List<Confluent.Kafka.TopicPartition>>();

        _workerPoolMock
            .Setup(x => x.StartAsync(partitions, It.IsAny<int>()))
            .Returns(Task.CompletedTask);

        _logHandlerMock
            .Setup(x => x.Info(It.IsAny<string>(), It.IsAny<object>()));

        _consumerMock.SetupGet(x=>x.Assignment).Returns(partitions.ToArray());

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
        _workerPoolMock.VerifyNoOtherCalls();
        _logHandlerMock.VerifyAll();
    }

    [TestMethod]
    public void OnPartitionsRevoked_ShouldCancelFeederBeforeWorkerPool_ToPreventOffsetLossUnderHighLoad()
    {
        // Arrange
        // Cancel() is used instead of StopAsync() because the callback runs on the same
        // thread as consumer.Consume() (the feeder thread). Awaiting the feeder task from
        // there would deadlock. Cancel() just signals the cancellation token; the feeder
        // exits naturally after Consume() returns.
        var partitions = _fixture.Create<List<Confluent.Kafka.TopicPartitionOffset>>();

        var callOrder = new List<string>();

        _feederMock
            .Setup(x => x.Cancel())
            .Callback(() => callOrder.Add("Feeder.Cancel"));

        _workerPoolMock
            .Setup(x => x.StopAsync())
            .Callback(() => callOrder.Add("WorkerPool.StopAsync"))
            .Returns(Task.CompletedTask);

        _logHandlerMock
            .Setup(x => x.Warning(It.IsAny<string>(), It.IsAny<object>()));

        // Act
        _onPartitionRevokedHandler(_dependencyResolver.Object, null, partitions);

        // Assert - feeder cancelled before pool drain
        _feederMock.Verify(x => x.Cancel(), Times.Once);

        Assert.AreEqual(2, callOrder.Count);
        Assert.AreEqual("Feeder.Cancel", callOrder[0]);
        Assert.AreEqual("WorkerPool.StopAsync", callOrder[1]);
    }

    [TestMethod]
    public void OnPartitionsAssigned_ShouldCancelAndRestartFeeder_ToCloseRebalanceRaceWindow()
    {
        // Arrange
        // OnPartitionAssigned must cancel any running feeder (including the one started at
        // startup) before the pool is rebuilt, then restart it after. This closes the window
        // where a stale message from a no-longer-assigned partition could be enqueued without
        // offset tracking. For stop-the-world strategy the pool is NOT stopped here (it was
        // already stopped in OnPartitionsRevoked).
        var partitions = _fixture.Create<List<Confluent.Kafka.TopicPartition>>();

        var callOrder = new List<string>();

        _feederMock
            .Setup(x => x.Cancel())
            .Callback(() => callOrder.Add("Feeder.Cancel"));

        _feederMock
            .Setup(x => x.Start())
            .Callback(() => callOrder.Add("Feeder.Start"));

        _workerPoolMock
            .Setup(x => x.StartAsync(partitions, It.IsAny<int>()))
            .Callback(() => callOrder.Add("WorkerPool.StartAsync"))
            .Returns(Task.CompletedTask);

        _logHandlerMock
            .Setup(x => x.Info(It.IsAny<string>(), It.IsAny<object>()));

        _consumerMock.SetupGet(x => x.Assignment).Returns(partitions.ToArray());

        // Act
        _onPartitionAssignedHandler(_dependencyResolver.Object, Mock.Of<Confluent.Kafka.IConsumer<byte[], byte[]>>(), partitions);

        // Assert - order: cancel → start pool → start feeder
        _feederMock.Verify(x => x.Cancel(), Times.Once);
        _feederMock.Verify(x => x.Start(), Times.Once);

        Assert.AreEqual(3, callOrder.Count);
        Assert.AreEqual("Feeder.Cancel", callOrder[0]);
        Assert.AreEqual("WorkerPool.StartAsync", callOrder[1]);
        Assert.AreEqual("Feeder.Start", callOrder[2]);
    }
}
