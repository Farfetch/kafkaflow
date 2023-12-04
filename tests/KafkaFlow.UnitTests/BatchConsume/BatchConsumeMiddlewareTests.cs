using System;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using KafkaFlow.Batching;
using KafkaFlow.Configuration;
using KafkaFlow.Consumers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace KafkaFlow.UnitTests.BatchConsume;

[TestClass]
public class BatchConsumeMiddlewareTests
{
    private const int BatchSize = 3;

    private readonly TimeSpan _batchTimeout = TimeSpan.FromMilliseconds(1000);
    private readonly TimeSpan _waitForTaskExecution = TimeSpan.FromMilliseconds(100);
    private readonly Fixture _fixture = new();

    private Mock<ILogHandler> _logHandlerMock;

    private IMessageContext _nextContext;
    private int _timesNextWasCalled;

    private BatchConsumeMiddleware _target;

    [TestInitialize]
    public void Setup()
    {
        _nextContext = null;
        _timesNextWasCalled = 0;

        _logHandlerMock = new Mock<ILogHandler>();

        var middlewareContextMock = new Mock<IConsumerMiddlewareContext>();
        var workerMock = new Mock<IWorker>();
        var consumerMock = new Mock<IConsumer>();
        var consumerConfigurationMock = new Mock<IConsumerConfiguration>();

        var clusterConfig = _fixture.Create<ClusterConfiguration>();

        consumerConfigurationMock.SetupGet(x => x.ClusterConfiguration).Returns(clusterConfig);

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
            .Returns(new Event(_logHandlerMock.Object));

        consumerConfigurationMock
            .SetupGet(x => x.AutoMessageCompletion)
            .Returns(true);

        _target = new BatchConsumeMiddleware(
            middlewareContextMock.Object,
            BatchSize,
            _batchTimeout,
            _logHandlerMock.Object);
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
        await _target.Invoke(context.Object, this.NextCallback);

        // Assert
        _timesNextWasCalled.Should().Be(0);
        await this.WaitBatchTimeoutAsync();
        _timesNextWasCalled.Should().Be(1);
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
            await _target.Invoke(contextMock.Object, this.NextCallback);
        }

        await Task.Delay(_waitForTaskExecution);

        // Assert
        _timesNextWasCalled.Should().Be(1);
        _nextContext.GetMessagesBatch().Should().HaveCount(BatchSize);
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
            await _target.Invoke(contextMock.Object, this.NextCallback);
        }

        await Task.Delay(_waitForTaskExecution);

        // Assert
        _timesNextWasCalled.Should().Be(1);
        _nextContext.GetMessagesBatch().Should().HaveCount(BatchSize);
        consumerContext.Verify(x => x.Complete(), Times.Exactly(BatchSize));

        await this.WaitBatchTimeoutAsync();
        _timesNextWasCalled.Should().Be(2);
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
        await _target.Invoke(contextMock.Object, _ => throw new Exception());

        // Assert
        await this.WaitBatchTimeoutAsync();
        _logHandlerMock.Verify(x => x.Error(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<object>()), Times.Once);
    }

    private Task WaitBatchTimeoutAsync() => Task.Delay(_batchTimeout + _waitForTaskExecution);

    private Task NextCallback(IMessageContext ctx)
    {
        _nextContext = ctx;
        _timesNextWasCalled++;
        return Task.CompletedTask;
    }
}
