using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using KafkaFlow.Admin;
using KafkaFlow.Admin.Messages;
using KafkaFlow.Admin.WebApi.Contracts;
using KafkaFlow.Admin.WebApi.Controllers;
using KafkaFlow.Consumers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace KafkaFlow.UnitTests.Admin.WebApi.Controllers;

[TestClass]
public class TelemetryControllerTests
{
    private Mock<ITelemetryStorage> _mockTelemetryStorage;
    private TelemetryController _target;

    [TestInitialize]
    public void TestInitialize()
    {
        _mockTelemetryStorage = new Mock<ITelemetryStorage>();
        _target = new TelemetryController(_mockTelemetryStorage.Object);
    }

    [TestMethod]
    public void GetTelemetry_ReturnsOkResultWithTelemetryResponse()
    {
        // Arrange
        var metrics = new List<ConsumerTelemetryMetric>
        {
            new ConsumerTelemetryMetric
            {
                GroupId = "group1",
                ConsumerName = "consumer1",
                WorkersCount = 5,
                SentAt = new DateTime(2023, 7, 28, 8, 20, 30),
                Topic = "topic1",
                Status = ConsumerStatus.Running,
                PausedPartitions = new[] { 0 },
                RunningPartitions = new[] { 10 },
                Lag = 20,
                InstanceName = "instanceName1",
            },
            new ConsumerTelemetryMetric
            {
                GroupId = "group1",
                ConsumerName = "consumer1",
                WorkersCount = 5,
                SentAt = new DateTime(2023, 7, 29, 1, 20, 30),
                Topic = "topic2",
                Status = ConsumerStatus.Paused,
                PausedPartitions = new[] { 5 },
                RunningPartitions = new[] { 0 },
                Lag = 0,
                InstanceName = "instanceName2",
            },
            new ConsumerTelemetryMetric
            {
                GroupId = "group1",
                ConsumerName = "consumer2",
                WorkersCount = 3,
                SentAt = new DateTime(2023, 6, 2, 10, 20, 30),
                Topic = "topic3",
                Status = ConsumerStatus.Stopped,
                PausedPartitions = new[] { 5 },
                RunningPartitions = new[] { 0 },
                Lag = 15,
                InstanceName = "instanceName3",
            },
        };

        _mockTelemetryStorage.Setup(x => x.Get()).Returns(metrics);

        // Act
        var result = _target.GetTelemetry() as ObjectResult;

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);

        var response = result.Value.Should().BeOfType<TelemetryResponse>().Subject;
        response.Groups.Should().HaveCount(1);

        var group = response.Groups.ElementAt(0);
        group.GroupId.Should().Be("group1");
        group.Consumers.Should().HaveCount(2);

        var consumer1 = group.Consumers.ElementAt(0);
        consumer1.Name.Should().Be("consumer1");
        consumer1.WorkersCount.Should().Be(5);
        consumer1.Assignments.Should().HaveCount(2);

        var assignment1 = consumer1.Assignments.ElementAt(0);
        assignment1.InstanceName.Should().NotBeNullOrEmpty();
        assignment1.TopicName.Should().Be("topic1");
        assignment1.Status.Should().Be("Running");
        assignment1.LastUpdate.Should().BeCloseTo(new DateTime(2023, 7, 28, 8, 20, 30), TimeSpan.FromSeconds(1));
        assignment1.PausedPartitions.Should().BeEquivalentTo(new[] { 0 });
        assignment1.RunningPartitions.Should().BeEquivalentTo(new[] { 10 });
        assignment1.Lag.Should().Be(20);

        var assignment2 = consumer1.Assignments.ElementAt(1);
        assignment2.InstanceName.Should().NotBeNullOrEmpty();
        assignment2.TopicName.Should().Be("topic2");
        assignment2.Status.Should().Be("Paused");
        assignment2.LastUpdate.Should().BeCloseTo(new DateTime(2023, 7, 29, 1, 20, 30), TimeSpan.FromSeconds(1));
        assignment2.PausedPartitions.Should().BeEquivalentTo(new[] { 5 });
        assignment2.RunningPartitions.Should().BeEquivalentTo(new[] { 0 });
        assignment2.Lag.Should().Be(0);

        var consumer2 = group.Consumers.ElementAt(1);
        consumer2.Name.Should().Be("consumer2");
        consumer2.WorkersCount.Should().Be(3);
        consumer2.Assignments.Should().HaveCount(1);

        var assignment3 = consumer2.Assignments.ElementAt(0);
        assignment3.InstanceName.Should().NotBeNullOrEmpty();
        assignment3.TopicName.Should().Be("topic3");
        assignment3.Status.Should().Be("Stopped");
        assignment3.LastUpdate.Should().BeCloseTo(new DateTime(2023, 6, 2, 10, 20, 30), TimeSpan.FromSeconds(1));
        assignment3.PausedPartitions.Should().BeEquivalentTo(new[] { 5 });
        assignment3.RunningPartitions.Should().BeEquivalentTo(new[] { 0 });
        assignment3.Lag.Should().Be(15);
    }
}
