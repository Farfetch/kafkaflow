using System;
using AutoFixture;
using Confluent.Kafka;
using AwesomeAssertions;
using KafkaFlow.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace KafkaFlow.UnitTests.ConfigurationBuilders;

[TestClass]
public class ProducerConfigurationBuilderTests
{
    private readonly Fixture _fixture = new();

    private Mock<IDependencyConfigurator> _dependencyConfiguratorMock;

    private string _name;

    private ProducerConfigurationBuilder _target;

    [TestInitialize]
    public void Setup()
    {
        _dependencyConfiguratorMock = new Mock<IDependencyConfigurator>();
        _name = _fixture.Create<string>();

        _target = new ProducerConfigurationBuilder(
            _dependencyConfiguratorMock.Object,
            _name);
    }

    [TestMethod]
    public void DependencyConfigurator_SetProperty_ReturnPassedInstance()
    {
        // Assert
        _target.DependencyConfigurator.Should().Be(_dependencyConfiguratorMock.Object);
    }

    [TestMethod]
    public void Build_RequiredCalls_ReturnDefaultValues()
    {
        // Arrange
        var clusterConfiguration = _fixture.Create<ClusterConfiguration>();

        // Act
        var configuration = _target.Build(clusterConfiguration);

        // Assert
        configuration.Cluster.Should().Be(clusterConfiguration);
        configuration.Name.Should().Be(_name);
        configuration.DefaultTopic.Should().BeNull();
        configuration.Acks.Should().BeNull();
        configuration.StatisticsHandlers.Should().BeEmpty();
        configuration.MiddlewaresConfigurations.Should().BeEmpty();
    }

    [TestMethod]
    public void Build_AllCalls_ReturnPassedValues()
    {
        // Arrange
        var clusterConfiguration = _fixture.Create<ClusterConfiguration>();

        var defaultTopic = _fixture.Create<string>();
        var acks = _fixture.Create<KafkaFlow.Acks>();
        const int lingerMs = 50;
        ProducerCustomFactory customFactory = (producer, _) => producer;
        Action<string> statisticsHandler = _ => { };
        const int statisticsIntervalMs = 100;
        var producerConfig = new ProducerConfig();
        var compressionType = CompressionType.Lz4;
        var compressionLevel = 5;

        _target
            .DefaultTopic(defaultTopic)
            .WithAcks(acks)
            .WithLingerMs(lingerMs)
            .WithCustomFactory(customFactory)
            .WithStatisticsHandler(statisticsHandler)
            .WithStatisticsIntervalMs(statisticsIntervalMs)
            .WithProducerConfig(producerConfig)
            .WithCompression(compressionType, compressionLevel)
            .AddMiddlewares(m => m.Add<IMessageMiddleware>());

        // Act
        var configuration = _target.Build(clusterConfiguration);

        // Assert
        configuration.Cluster.Should().Be(clusterConfiguration);
        configuration.Name.Should().Be(_name);
        configuration.DefaultTopic.Should().Be(defaultTopic);
        configuration.Acks.Should().Be(acks);
        configuration.BaseProducerConfig.LingerMs.Should().Be(lingerMs);
        configuration.BaseProducerConfig.CompressionType.Should().Be(compressionType);
        configuration.BaseProducerConfig.CompressionLevel.Should().Be(compressionLevel);
        configuration.BaseProducerConfig.StatisticsIntervalMs.Should().Be(statisticsIntervalMs);
        configuration.StatisticsHandlers.Should().HaveElementAt(0, statisticsHandler);
        configuration.BaseProducerConfig.Should().BeSameAs(producerConfig);
        configuration.MiddlewaresConfigurations.Should().HaveCount(1);
    }

    [TestMethod]
    public void Build_UseCompressionWithoutCompressionLevel_ReturnDefaultValues()
    {
        // Arrange
        var clusterConfiguration = _fixture.Create<ClusterConfiguration>();

        var compressionType = CompressionType.Gzip;

        _target
            .WithCompression(compressionType);

        // Act
        var configuration = _target.Build(clusterConfiguration);

        // Assert
        configuration.Cluster.Should().Be(clusterConfiguration);
        configuration.Name.Should().Be(_name);
        configuration.BaseProducerConfig.CompressionType.Should().Be(compressionType);
        configuration.BaseProducerConfig.CompressionLevel.Should().Be(-1);
    }

    [TestMethod]
    public void Build_UseProducerConfig_ShouldNotOverrideLingerMsAndStatisticsIntervalMs()
    {
        // Arrange
        var clusterConfiguration = _fixture.Create<ClusterConfiguration>();

        const double lingerMs = 1000;
        const int statisticsMs = 2000;
        var producerConfig = new ProducerConfig
        {
            LingerMs = lingerMs,
            StatisticsIntervalMs = statisticsMs
        };

        _target.WithProducerConfig(producerConfig);

        // Act
        var configuration = _target.Build(clusterConfiguration);

        // Assert
        configuration.Cluster.Should().Be(clusterConfiguration);
        configuration.BaseProducerConfig.LingerMs.Should().Be(lingerMs);
        configuration.BaseProducerConfig.StatisticsIntervalMs.Should().Be(statisticsMs);
    }

    [TestMethod]
    public void Build_UseProducerConfigAndUseLingerMs_ShouldOverrideLingerMs()
    {
        // Arrange
        var clusterConfiguration = _fixture.Create<ClusterConfiguration>();

        const double producerConfigLingerMs = 1000;
        const double expectedLingerMs = 2000;
        var producerConfig = new ProducerConfig { LingerMs = producerConfigLingerMs };

        _target
            .WithProducerConfig(producerConfig)
            .WithLingerMs(expectedLingerMs);

        // Act
        var configuration = _target.Build(clusterConfiguration);

        // Assert
        configuration.Cluster.Should().Be(clusterConfiguration);
        configuration.BaseProducerConfig.LingerMs.Should().Be(expectedLingerMs);
    }

    [TestMethod]
    public void Build_UseProducerConfigAndUseStatisticsIntervalMs_ShouldOverrideStatisticsIntervalMs()
    {
        // Arrange
        var clusterConfiguration = _fixture.Create<ClusterConfiguration>();

        const int producerConfigStatisticsIntervalMs = 1000;
        const int expectedStatisticsIntervalMs = 2000;
        var producerConfig = new ProducerConfig
        {
            StatisticsIntervalMs = producerConfigStatisticsIntervalMs
        };

        _target
            .WithProducerConfig(producerConfig)
            .WithStatisticsIntervalMs(expectedStatisticsIntervalMs);

        // Act
        var configuration = _target.Build(clusterConfiguration);

        // Assert
        configuration.Cluster.Should().Be(clusterConfiguration);
        configuration
            .BaseProducerConfig.StatisticsIntervalMs.Should()
            .Be(expectedStatisticsIntervalMs);
    }
}
