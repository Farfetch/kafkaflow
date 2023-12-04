using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using KafkaFlow.Admin;
using KafkaFlow.Admin.WebApi.Contracts;
using KafkaFlow.Admin.WebApi.Controllers;
using KafkaFlow.Consumers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace KafkaFlow.UnitTests.Admin.WebApi.Controllers;

[TestClass]
public class GroupsControllerTests
{
    private readonly Fixture _fixture = new();
    private GroupsController _target;
    private Mock<IConsumerAccessor> _mockConsumerAccessor;
    private Mock<IConsumerAdmin> _mockConsumerAdmin;

    [TestInitialize]
    public void TestSetup()
    {
        _mockConsumerAccessor = _fixture.Freeze<Mock<IConsumerAccessor>>();
        _mockConsumerAdmin = _fixture.Freeze<Mock<IConsumerAdmin>>();
        _target = new GroupsController(_mockConsumerAccessor.Object, _mockConsumerAdmin.Object);
    }

    [TestMethod]
    public void GetAllGroups_ReturnsOkResultWithGroupsResponse()
    {
        // Arrange
        var consumers = new List<IMessageConsumer>
        {
            Mock.Of<IMessageConsumer>(),
            Mock.Of<IMessageConsumer>(),
            Mock.Of<IMessageConsumer>(),
        };

        _mockConsumerAccessor.Setup(x => x.All).Returns(consumers);

        // Act
        var result = _target.GetAllGroups() as ObjectResult;

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);

        var response = result.Value.Should().BeOfType<GroupsResponse>().Subject;
        response.Groups.Should().HaveCount(1);
        response.Groups.First().Consumers.Should().HaveCount(3);
    }

    [TestMethod]
    public async Task PauseGroup_ValidGroupId_ReturnsAcceptedResult()
    {
        // Arrange
        var groupId = "group1";
        var topics = new List<string> { "topic1", "topic2" };

        // Act
        var result = await _target.PauseGroup(groupId, topics) as AcceptedResult;

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(202);

        _mockConsumerAdmin.Verify(x => x.PauseConsumerGroupAsync(groupId, topics), Times.Once);
    }

    [TestMethod]
    public async Task ResumeGroup_ValidGroupId_ReturnsAcceptedResult()
    {
        // Arrange
        var groupId = "group1";
        var topics = new List<string> { "topic1", "topic2" };

        // Act
        var result = await _target.ResumeGroup(groupId, topics) as AcceptedResult;

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(202);

        _mockConsumerAdmin.Verify(x => x.ResumeConsumerGroupAsync(groupId, topics), Times.Once);
    }
}
