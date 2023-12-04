using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace KafkaFlow.UnitTests.LogHandlers;

[TestClass]
public class MicrosoftLogHandlerTests
{
    [TestMethod]
    public void Constructor_CreatesNamedLogger()
    {
        // Arrange
        var loggerMock = new Mock<ILogger>();
        var loggerFactoryMock = new Mock<ILoggerFactory>();
        loggerFactoryMock.Setup(x => x.CreateLogger("KafkaFlow")).Returns(loggerMock.Object);

        // Act
        new MicrosoftLogHandler(loggerFactoryMock.Object);

        // Assert
        loggerFactoryMock.Verify(x => x.CreateLogger("KafkaFlow"), Times.Once);
    }
}
