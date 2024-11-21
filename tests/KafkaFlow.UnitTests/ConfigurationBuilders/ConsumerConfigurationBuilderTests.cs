using System;
using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using KafkaFlow.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace KafkaFlow.UnitTests.ConfigurationBuilders;

[TestClass]
public class ConsumerConfigurationBuilderTests
{
    private readonly Fixture _fixture = new();

    private Mock<IDependencyConfigurator> _dependencyConfiguratorMock;

    private ConsumerConfigurationBuilder _target;

    [TestInitialize]
    public void Setup()
    {
        _dependencyConfiguratorMock = new Mock<IDependencyConfigurator>();

        _target = new ConsumerConfigurationBuilder(_dependencyConfiguratorMock.Object);
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
        var topic1 = _fixture.Create<string>();
        const int bufferSize = 100;
        const int workers = 10;
        var groupId = _fixture.Create<string>();

        _target
            .Topics(topic1)
            .WithBufferSize(bufferSize)
            .WithWorkersCount(workers)
            .WithGroupId(groupId);

        // Act
        var configuration = _target.Build(clusterConfiguration);

        // Assert
        configuration.Topics.Should().BeEquivalentTo(topic1);
        configuration.BufferSize.Should().Be(bufferSize);
        configuration.WorkersCountCalculator(null, null).Result.Should().Be(workers);
        configuration.GroupId.Should().Be(groupId);
        configuration.GetKafkaConfig().AutoOffsetReset.Should().BeNull();
        configuration.GetKafkaConfig().EnableAutoOffsetStore.Should().Be(false);
        configuration.GetKafkaConfig().EnableAutoCommit.Should().Be(false);
        configuration.AutoMessageCompletion.Should().Be(true);
        configuration.AutoCommitInterval.Should().Be(TimeSpan.FromSeconds(5));
        configuration.StatisticsHandlers.Should().BeEmpty();
        configuration.PartitionsAssignedHandlers.Should().BeEmpty();
        configuration.PartitionsRevokedHandlers.Should().BeEmpty();
        configuration.MiddlewaresConfigurations.Should().BeEmpty();
    }

    [TestMethod]
    public void Build_AllCalls_ReturnPassedValues()
    {
        // Arrange
        var clusterConfiguration = _fixture.Create<ClusterConfiguration>();
        var topic1 = _fixture.Create<string>();
        var topic2 = _fixture.Create<string>();
        var name = _fixture.Create<string>();
        const int bufferSize = 100;
        const int workers = 10;
        const AutoOffsetReset offsetReset = AutoOffsetReset.Earliest;
        var groupId = _fixture.Create<string>();
        const int autoCommitInterval = 10000;
        const int maxPollIntervalMs = 500000;
        ConsumerCustomFactory customFactory = (producer, _) => producer;
        Action<string> statisticsHandler = _ => { };
        Action<IDependencyResolver, List<Confluent.Kafka.TopicPartition>> partitionsAssignedHandler = (_, _) => { };
        Action<IDependencyResolver, List<Confluent.Kafka.TopicPartitionOffset>> partitionsRevokedHandler = (_, _) => { };
        const int statisticsIntervalMs = 100;
        var consumerConfig = new Confluent.Kafka.ConsumerConfig
        {
            ClientId = "testeclient",
        };

        _target
            .Topics(topic1)
            .Topic(topic2)
            .WithName(name)
            .WithBufferSize(bufferSize)
            .WithWorkersCount(workers)
            .WithGroupId(groupId)
            .WithAutoOffsetReset(offsetReset)
            .WithManualMessageCompletion()
            .WithAutoCommitIntervalMs(autoCommitInterval)
            .WithMaxPollIntervalMs(maxPollIntervalMs)
            .WithConsumerConfig(consumerConfig)
            .WithCustomFactory(customFactory)
            .WithStatisticsIntervalMs(statisticsIntervalMs)
            .WithStatisticsHandler(statisticsHandler)
            .WithPartitionsAssignedHandler(partitionsAssignedHandler)
            .WithPartitionsRevokedHandler(partitionsRevokedHandler)
            .AddMiddlewares(m => m.Add<IMessageMiddleware>());

        // Act
        var configuration = _target.Build(clusterConfiguration);

        // Assert
        configuration.Topics.Should().BeEquivalentTo(topic1, topic2);
        configuration.ConsumerName.Should().Be(name);
        configuration.BufferSize.Should().Be(bufferSize);
        configuration.WorkersCountCalculator(null, null).Result.Should().Be(workers);
        configuration.GroupId.Should().Be(groupId);
        configuration.GetKafkaConfig().AutoOffsetReset.Should().HaveSameValueAs(offsetReset);
        configuration.AutoMessageCompletion.Should().Be(false);
        configuration.GetKafkaConfig().EnableAutoOffsetStore.Should().Be(false);
        configuration.GetKafkaConfig().EnableAutoCommit.Should().Be(false);
        configuration.AutoCommitInterval.Should().Be(TimeSpan.FromMilliseconds(autoCommitInterval));
        configuration.GetKafkaConfig().MaxPollIntervalMs.Should().Be(maxPollIntervalMs);
        configuration.GetKafkaConfig().StatisticsIntervalMs.Should().Be(statisticsIntervalMs);
        configuration.StatisticsHandlers.Should().HaveElementAt(0, statisticsHandler);
        configuration.PartitionsAssignedHandlers.Should().HaveElementAt(0, partitionsAssignedHandler);
        configuration.PartitionsRevokedHandlers.Should().HaveElementAt(0, partitionsRevokedHandler);
        configuration.GetKafkaConfig().ClientId.Should().Be(consumerConfig.ClientId);
        configuration.MiddlewaresConfigurations.Should().HaveCount(1);
    }
}
