using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using KafkaFlow.Consumers.DistributionStrategies;
using KafkaFlow.UnitTests.Factories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KafkaFlow.UnitTests.Consumer.DistributionStrategies;

[TestClass]
public class BytesSumDistributionStrategyTests
{
    [TestMethod]
    public async Task GetWorkerAsync_SingleWorkerRegistered_ReturnsThatWorkerIndepedentOfPartitionKey()
    {
        // Arrange
        var expectedWorkerId = 0;
        var workers = WorkerFactory.CreateWorkers(1);

        var target = new BytesSumDistributionStrategy();
        target.Initialize(workers);

        // Act
        var worker = await target.GetWorkerAsync(
            new WorkerDistributionContext(null, null, 100, null, CancellationToken.None));

        // Assert
        worker.Id.Should().Be(expectedWorkerId);
    }

    [TestMethod]
    public async Task GetWorkerAsync_MessageKeyNull_ReturnsFirstWorker()
    {
        // Arrange
        var workers = WorkerFactory.CreateWorkers(10);

        var target = new BytesSumDistributionStrategy();
        target.Initialize(workers);

        // Act
        var worker = await target.GetWorkerAsync(
            new WorkerDistributionContext(null, null, 100, null, CancellationToken.None));

        // Assert
        worker.Id.Should().Be(workers.First().Id);
    }

    [TestMethod]
    public async Task GetWorkerAsync_CancellationRequested_ReturnsNull()
    {
        // Arrange
        var workers = WorkerFactory.CreateWorkers(10);

        var target = new BytesSumDistributionStrategy();
        target.Initialize(workers);

        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        var worker = await target.GetWorkerAsync(
            new WorkerDistributionContext(null, null, 0, Encoding.ASCII.GetBytes("key"), cts.Token));

        // Assert
        worker.Should().BeNull();
    }

    [TestMethod]
    public async Task GetWorkerAsync_ListOfWorkers_ReturnsExpectedWorkerForMessageKey()
    {
        // Arrange
        var workerCount = 10;

        // If the messageKey is changed the byteSum value needs to be updated
        var messageKey = new ReadOnlyMemory<byte>(Encoding.ASCII.GetBytes("key"));
        var byteSum = 329;
        var expectedWorkerId = byteSum % workerCount;
        var workers = WorkerFactory.CreateWorkers(workerCount);

        var target = new BytesSumDistributionStrategy();
        target.Initialize(workers);

        // Act
        var worker = await target.GetWorkerAsync(
            new WorkerDistributionContext(null, null, 0, messageKey, CancellationToken.None));

        // Assert
        worker.Id.Should().Be(expectedWorkerId);
    }
}
