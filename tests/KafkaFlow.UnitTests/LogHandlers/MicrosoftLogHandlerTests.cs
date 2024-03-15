using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;

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

    [TestMethod]
    public void LogWarning_WithNotSerializableData_DoesNotSerializeObject()
    {
        // Arrange
        var expectedMessage = "Any message";
        var expectedSerializationErrorMessage = "Log data serialization error.";
        var loggerMock = new Mock<ILogger>();
        var loggerFactoryMock = new Mock<ILoggerFactory>();
        
        loggerFactoryMock
            .Setup(x => x.CreateLogger("KafkaFlow"))
            .Returns(loggerMock.Object);

        var handler = new MicrosoftLogHandler(loggerFactoryMock.Object);

        // Act
        handler.Warning(expectedMessage, new
        {
            In = new IntPtr(1)
        });

        // Assert
        Assert.AreEqual(1, loggerMock.Invocations.Count);
        Assert.IsTrue(loggerMock.Invocations[0].Arguments.Any(x => x.ToString().Contains(expectedMessage)));
        Assert.IsTrue(loggerMock.Invocations[0].Arguments.Any(x => x.ToString().Contains(expectedSerializationErrorMessage)));
    }
}
