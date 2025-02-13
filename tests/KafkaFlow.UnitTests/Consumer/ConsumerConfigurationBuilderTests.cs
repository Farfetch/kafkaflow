using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using Confluent.Kafka;
using FluentAssertions;
using KafkaFlow.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace KafkaFlow.UnitTests.Consumer;

[TestClass]
public class ConsumerConfigurationBuilderTests
{
    private readonly Fixture _fixture = new();

    [TestInitialize]
    public void Setup()
    {
        _fixture.Customize(new AutoMoqCustomization());
    }

    [TestMethod]
    public void ConfigurationBuild_CallBuild_WithSticky_EnableAutoCommit_True()
    {
        // Arrange
        var consumerConfigurationBuilder = _fixture.Create<ConsumerConfigurationBuilder>();
        consumerConfigurationBuilder.WithConsumerConfig(new ConsumerConfig
            {
            PartitionAssignmentStrategy = PartitionAssignmentStrategy.CooperativeSticky,
            GroupId = "Test",
        }).WithAutoCommitIntervalMs(500)
            .WithBufferSize(3);
        
        // Act
        var consumerConfiguration = consumerConfigurationBuilder.Build(_fixture.Create<ClusterConfiguration>());

        // Assert
        var consumerConfig = consumerConfiguration.GetKafkaConfig();
        consumerConfig.EnableAutoCommit.Should().BeTrue();
        consumerConfig.AutoCommitIntervalMs.Should().Be(500);
        consumerConfiguration.AutoCommitInterval.Should().Be(TimeSpan.FromMilliseconds(500));
    }

    [TestMethod]
    public void ConfigurationBuild_CallBuild_WithSRoundRobin_EnableAutoCommit_False()
    {
        // Arrange
        var consumerConfigurationBuilder = new ConsumerConfigurationBuilder(Mock.Of<IDependencyConfigurator>());
        consumerConfigurationBuilder.WithConsumerConfig(new ConsumerConfig
        {
            PartitionAssignmentStrategy = PartitionAssignmentStrategy.RoundRobin,
            GroupId = "Test"
        }).WithAutoCommitIntervalMs(500).WithBufferSize(3);
        // Act
        var consumerConfiguration = consumerConfigurationBuilder.Build(_fixture.Create<ClusterConfiguration>());

        // Assert
        consumerConfiguration.GetKafkaConfig().EnableAutoCommit.Should().BeFalse();
        consumerConfiguration.AutoCommitInterval.Should().Be(TimeSpan.FromMilliseconds(500));
    }
}