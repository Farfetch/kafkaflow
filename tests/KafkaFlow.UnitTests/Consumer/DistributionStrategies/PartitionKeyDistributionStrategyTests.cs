using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using KafkaFlow.Consumers.DistributionStrategies;
using KafkaFlow.UnitTests.Factories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KafkaFlow.UnitTests.Consumer.DistributionStrategies;

[TestClass]
public class PartitionKeyDistributionStrategyTests
{
    [TestMethod]
    public async Task GetWorkerAsync_SingleWorkerRegistered_ReturnsThatWorkerIndepedentOfPartitionKey()
    {
        // Arrange
        var expectedWorkerId = 0;
        var workers = WorkerFactory.CreateWorkers(1);

        var target = new PartitionKeyDistributionStrategy();
        target.Initialize(workers);

        // Act
        var worker = await target.GetWorkerAsync(
            new WorkerDistributionContext(null, null, 100, null, CancellationToken.None));

        // Assert
        worker.Id.Should().Be(expectedWorkerId);
    }

    [TestMethod]
    public async Task GetWorkerAsync_CancellationRequested_ReturnsNull()
    {
        // Arrange
        var workers = WorkerFactory.CreateWorkers(10);

        var target = new PartitionKeyDistributionStrategy();
        target.Initialize(workers);

        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        var worker = await target.GetWorkerAsync(
            new WorkerDistributionContext(null, null, 0, null, cts.Token));

        // Assert
        worker.Should().BeNull();
    }

    [TestMethod]
    public async Task GetWorkerAsync_ListOfWorkers_ReturnsExpectedWorkerForPartitionKey()
    {
        // Arrange
        var workerCount = 10;
        var partition = 5;
        var expectedWorkerId = partition % workerCount;
        var workers = WorkerFactory.CreateWorkers(workerCount);

        var target = new PartitionKeyDistributionStrategy();
        target.Initialize(workers);

        // Act
        var worker = await target.GetWorkerAsync(
            new WorkerDistributionContext(null, null, partition, null, CancellationToken.None));

        // Assert
        worker.Id.Should().Be(expectedWorkerId);
    }
}
