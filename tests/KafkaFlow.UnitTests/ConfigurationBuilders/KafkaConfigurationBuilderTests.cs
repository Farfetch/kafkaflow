﻿using KafkaFlow.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace KafkaFlow.UnitTests.ConfigurationBuilders;

[TestClass]
public class KafkaConfigurationBuilderTests
{
    [TestMethod]
    public void ExtensionMethod_UseMicrosoftLog_ConfigureMicrosoftLogHandler()
    {
        // Arrange
        var builder = new Mock<IKafkaConfigurationBuilder>();

        // Act
        ExtensionMethods.UseMicrosoftLog(builder.Object);

        // Assert
        builder.Verify(x => x.UseLogHandler<MicrosoftLogHandler>(), Times.Once);
    }
}
