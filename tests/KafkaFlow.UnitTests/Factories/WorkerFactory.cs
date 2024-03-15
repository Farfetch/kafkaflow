using Moq;

namespace KafkaFlow.UnitTests.Factories;

internal static class WorkerFactory
{
    /// <summary>
    /// Generate workers for testing. The Worker ID's are sequencial and zero-based.
    /// </summary>
    /// <param name="workerCount">Number of workers to generate</param>
    /// <returns>A list of workers</returns>
    public static IWorker[] CreateWorkers(int workerCount)
    {
        var workers = new IWorker[workerCount];

        for (var i = 0; i < workerCount; i++)
        {
            var workerMock = new Mock<IWorker>();
            workerMock.Setup(x => x.Id).Returns(i);

            workers[i] = workerMock.Object;
        }

        return workers;
    }
}
