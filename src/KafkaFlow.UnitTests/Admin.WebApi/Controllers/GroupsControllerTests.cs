namespace KafkaFlow.UnitTests.Admin.WebApi.Controllers
{
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

    [TestClass]
    public class GroupsControllerTests
    {
        private readonly Fixture fixture = new();
        private GroupsController target;
        private Mock<IConsumerAccessor> mockConsumerAccessor;
        private Mock<IConsumerAdmin> mockConsumerAdmin;

        [TestInitialize]
        public void TestSetup()
        {
            this.mockConsumerAccessor = this.fixture.Freeze<Mock<IConsumerAccessor>>();
            this.mockConsumerAdmin = this.fixture.Freeze<Mock<IConsumerAdmin>>();
            this.target = new GroupsController(this.mockConsumerAccessor.Object, this.mockConsumerAdmin.Object);
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

            this.mockConsumerAccessor.Setup(x => x.All).Returns(consumers);

            // Act
            var result = this.target.GetAllGroups() as ObjectResult;

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
            var result = await this.target.PauseGroup(groupId, topics) as AcceptedResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(202);

            this.mockConsumerAdmin.Verify(x => x.PauseConsumerGroupAsync(groupId, topics), Times.Once);
        }

        [TestMethod]
        public async Task ResumeGroup_ValidGroupId_ReturnsAcceptedResult()
        {
            // Arrange
            var groupId = "group1";
            var topics = new List<string> { "topic1", "topic2" };

            // Act
            var result = await this.target.ResumeGroup(groupId, topics) as AcceptedResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(202);

            this.mockConsumerAdmin.Verify(x => x.ResumeConsumerGroupAsync(groupId, topics), Times.Once);
        }
    }
}
