using System;
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

namespace KafkaFlow.UnitTests.Admin.WebApi.Controllers
{
    [TestClass]
    public class ConsumersControllerTests
    {
        private readonly Fixture _fixture = new();
        private ConsumersController _target;
        private Mock<IConsumerAccessor> _mockConsumerAccessor;
        private Mock<IConsumerAdmin> _mockConsumerAdmin;

        [TestInitialize]
        public void TestSetup()
        {
            _mockConsumerAccessor = _fixture.Freeze<Mock<IConsumerAccessor>>();
            _mockConsumerAdmin = _fixture.Freeze<Mock<IConsumerAdmin>>();
            _target = new ConsumersController(_mockConsumerAccessor.Object, _mockConsumerAdmin.Object);
        }

        [TestMethod]
        public void GetConsumersByGroupId_ValidGroupId_ReturnsOkResultWithConsumersResponse()
        {
            // Arrange
            var groupId = "group1";

            var consumers = new List<IMessageConsumer>
            {
                Mock.Of<IMessageConsumer>(c => c.GroupId == "group1" && c.ConsumerName == "consumer1"),
                Mock.Of<IMessageConsumer>(c => c.GroupId == "group1" && c.ConsumerName == "consumer2"),
            };

            _mockConsumerAccessor.Setup(x => x.All).Returns(consumers);

            // Act
            var result = _target.GetConsumersByGroupId(groupId) as ObjectResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);

            var response = result.Value.Should().BeOfType<ConsumersResponse>().Subject;
            response.Consumers.Should().HaveCount(2);
            response.Consumers.Select(c => c.ConsumerName).Should().Contain(new[] { "consumer1", "consumer2" });
        }

        [TestMethod]
        public void GetConsumerByGroupIdName_ValidGroupIdAndExistingConsumer_ReturnsOkResultWithConsumerResponse()
        {
            // Arrange
            var groupId = "group1";
            var consumerName = "consumer1";

            var consumers = new List<IMessageConsumer>
            {
                Mock.Of<IMessageConsumer>(c => c.GroupId == groupId && c.ConsumerName == consumerName),
                Mock.Of<IMessageConsumer>(c => c.GroupId == "group1" && c.ConsumerName == "consumer2"),
            };

            _mockConsumerAccessor.Setup(x => x.All).Returns(consumers.AsQueryable());

            // Act
            var result = _target.GetConsumerByGroupIdName(groupId, consumerName) as ObjectResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);

            var response = result.Value.Should().BeOfType<ConsumerResponse>().Subject;
            response.GroupId.Should().Be(groupId);
            response.ConsumerName.Should().Be(consumerName);
        }

        [TestMethod]
        public void GetConsumerByGroupIdName_ValidGroupIdAndNonExistingConsumer_ReturnsNotFoundResult()
        {
            // Arrange
            var groupId = "group1";
            var consumerName = "nonExistingConsumer";

            // Act
            var result = _target.GetConsumerByGroupIdName(groupId, consumerName) as NotFoundResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(404);
        }

        [TestMethod]
        public async Task PauseConsumer_ValidGroupIdAndExistingConsumer_ReturnsAcceptedResult()
        {
            // Arrange
            var groupId = "group1";
            var consumerName = "consumer1";
            var topics = new List<string> { "topic1", "topic2" };

            var consumers = new List<IMessageConsumer>
            {
                Mock.Of<IMessageConsumer>(c => c.GroupId == groupId && c.ConsumerName == consumerName),
            };

            _mockConsumerAccessor.Setup(x => x.All).Returns(consumers);

            // Act
            var result = await _target.PauseConsumer(groupId, consumerName, topics) as AcceptedResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(202);

            _mockConsumerAdmin.Verify(x => x.PauseConsumerAsync(consumerName, topics), Times.Once);
        }

        [TestMethod]
        public async Task PauseConsumer_ValidGroupIdAndNonExistingConsumer_ReturnsNotFoundResult()
        {
            // Arrange
            var groupId = "group1";
            var consumerName = "consumer1";
            var topics = new List<string> { "topic1", "topic2" };

            var consumers = new List<IMessageConsumer>
            {
                Mock.Of<IMessageConsumer>(c => c.GroupId == groupId && c.ConsumerName == "consumer2"),
            };

            _mockConsumerAccessor.Setup(x => x.All).Returns(consumers);

            // Act
            var result = await _target.PauseConsumer(groupId, consumerName, topics) as NotFoundResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(404);
        }

        [TestMethod]
        public async Task ResumeConsumer_ValidGroupIdAndNonExistingConsumer_ReturnsNotFoundResult()
        {
            // Arrange
            var groupId = "group1";
            var consumerName = "nonExistingConsumer";
            var topics = new List<string> { "topic1", "topic2" };

            var consumers = new List<IMessageConsumer>
            {
                Mock.Of<IMessageConsumer>(c => c.GroupId == groupId && c.ConsumerName == "existingConsumer"),
            };

            _mockConsumerAccessor.Setup(x => x.All).Returns(consumers);

            // Act
            var result = await _target.ResumeConsumer(groupId, consumerName, topics) as NotFoundResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(404);

            _mockConsumerAdmin.Verify(x => x.ResumeConsumerAsync(consumerName, topics), Times.Never);
        }

        [TestMethod]
        public async Task StartConsumer_ValidGroupIdAndExistingConsumer_ReturnsAcceptedResult()
        {
            // Arrange
            var groupId = "group1";
            var consumerName = "consumer1";

            var consumers = new List<IMessageConsumer>
            {
                Mock.Of<IMessageConsumer>(c => c.GroupId == groupId && c.ConsumerName == consumerName),
            };

            _mockConsumerAccessor.Setup(x => x.All).Returns(consumers);

            // Act
            var result = await _target.StartConsumer(groupId, consumerName) as AcceptedResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(202);

            _mockConsumerAdmin.Verify(x => x.StartConsumerAsync(consumerName), Times.Once);
        }

        [TestMethod]
        public async Task StartConsumer_ValidGroupIdAndNonExistingConsumer_ReturnsNotFoundResult()
        {
            // Arrange
            var groupId = "group1";
            var consumerName = "nonExistingConsumer";

            var consumers = new List<IMessageConsumer>
            {
                Mock.Of<IMessageConsumer>(c => c.GroupId == groupId && c.ConsumerName == "consumer1"),
            };

            _mockConsumerAccessor.Setup(x => x.All).Returns(consumers);

            // Act
            var result = await _target.StartConsumer(groupId, consumerName) as NotFoundResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(404);

            _mockConsumerAdmin.Verify(x => x.StartConsumerAsync(consumerName), Times.Never);
        }

        [TestMethod]
        public async Task StopConsumer_ValidGroupIdAndExistingConsumer_ReturnsAcceptedResult()
        {
            // Arrange
            var groupId = "group1";
            var consumerName = "consumer1";

            var consumers = new List<IMessageConsumer>
            {
                Mock.Of<IMessageConsumer>(c => c.GroupId == groupId && c.ConsumerName == consumerName),
            };

            _mockConsumerAccessor.Setup(x => x.All).Returns(consumers);

            // Act
            var result = await _target.StopConsumer(groupId, consumerName) as AcceptedResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(202);

            _mockConsumerAdmin.Verify(x => x.StopConsumerAsync(consumerName), Times.Once);
        }

        [TestMethod]
        public async Task StopConsumer_ValidGroupIdAndNonExistingConsumer_ReturnsNotFoundResult()
        {
            // Arrange
            var groupId = "group1";
            var consumerName = "nonExistingConsumer";

            var consumers = new List<IMessageConsumer>
            {
                Mock.Of<IMessageConsumer>(c => c.GroupId == groupId && c.ConsumerName == "consumer1"),
            };

            _mockConsumerAccessor.Setup(x => x.All).Returns(consumers);

            // Act
            var result = await _target.StopConsumer(groupId, consumerName) as NotFoundResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(404);

            _mockConsumerAdmin.Verify(x => x.StopConsumerAsync(consumerName), Times.Never);
        }

        [TestMethod]
        public async Task RestartConsumer_ValidGroupIdAndExistingConsumer_ReturnsAcceptedResult()
        {
            // Arrange
            var groupId = "group1";
            var consumerName = "consumer1";

            var consumers = new List<IMessageConsumer>
            {
                Mock.Of<IMessageConsumer>(c => c.GroupId == groupId && c.ConsumerName == consumerName),
            };

            _mockConsumerAccessor.Setup(x => x.All).Returns(consumers);

            // Act
            var result = await _target.RestartConsumer(groupId, consumerName) as AcceptedResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(202);

            _mockConsumerAdmin.Verify(x => x.RestartConsumerAsync(consumerName), Times.Once);
        }

        [TestMethod]
        public async Task RestartConsumer_ValidGroupIdAndNonExistingConsumer_ReturnsNotFoundResult()
        {
            // Arrange
            var groupId = "group1";
            var consumerName = "nonExistingConsumer";

            var consumers = new List<IMessageConsumer>
            {
                Mock.Of<IMessageConsumer>(c => c.GroupId == groupId && c.ConsumerName == "consumer1"),
            };

            _mockConsumerAccessor.Setup(x => x.All).Returns(consumers);

            // Act
            var result = await _target.RestartConsumer(groupId, consumerName) as NotFoundResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(404);

            _mockConsumerAdmin.Verify(x => x.StopConsumerAsync(consumerName), Times.Never);
        }

        [TestMethod]
        public async Task ResetOffsets_ValidRequest_ReturnsAcceptedResult()
        {
            // Arrange
            var groupId = "group1";
            var consumerName = "consumer1";
            var topics = new List<string> { "topic1", "topic2" };
            var request = new ResetOffsetsRequest { Confirm = true };

            var consumers = new List<IMessageConsumer>
            {
                Mock.Of<IMessageConsumer>(c => c.GroupId == groupId && c.ConsumerName == consumerName),
            };

            _mockConsumerAccessor.Setup(x => x.All).Returns(consumers);

            // Act
            var result = await _target.ResetOffsets(groupId, consumerName, topics, request) as AcceptedResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(202);

            _mockConsumerAdmin.Verify(x => x.ResetOffsetsAsync(consumerName, topics), Times.Once);
        }

        [TestMethod]
        public async Task ResetOffsets_InvalidConfirmValue_ReturnsBadRequestResult()
        {
            // Arrange
            var groupId = "group1";
            var consumerName = "consumer1";
            var topics = new List<string> { "topic1", "topic2" };
            var request = new ResetOffsetsRequest { Confirm = false }; // Invalid Confirm value

            var consumers = new List<IMessageConsumer>
            {
                Mock.Of<IMessageConsumer>(c => c.GroupId == groupId && c.ConsumerName == consumerName),
            };

            _mockConsumerAccessor.Setup(x => x.All).Returns(consumers);

            // Act
            var result = await _target.ResetOffsets(groupId, consumerName, topics, request) as BadRequestResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(400);

            _mockConsumerAdmin.Verify(x => x.ResetOffsetsAsync(consumerName, topics), Times.Never);
        }

        [TestMethod]
        public async Task RewindOffsets_ValidGroupIdAndNonExistingConsumer_ReturnsNotFoundResult()
        {
            // Arrange
            var groupId = "group1";
            var consumerName = "nonExistingConsumer";
            var topics = new List<string> { "topic1", "topic2" };
            var request = new RewindOffsetsToDateRequest { Date = _fixture.Create<DateTime>() };

            var consumers = new List<IMessageConsumer>
            {
                Mock.Of<IMessageConsumer>(c => c.GroupId == groupId && c.ConsumerName == "consumer1"),
            };

            _mockConsumerAccessor.Setup(x => x.All).Returns(consumers);

            // Act
            var result = await _target.RewindOffsets(groupId, consumerName, topics, request) as NotFoundResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(404);

            _mockConsumerAdmin.Verify(x => x.RewindOffsetsAsync(consumerName, request.Date, topics), Times.Never);
        }

        [TestMethod]
        public async Task RewindOffsets_ValidRequest_ReturnsAcceptedResult()
        {
            // Arrange
            var groupId = "group1";
            var consumerName = "consumer1";
            var topics = new List<string> { "topic1", "topic2" };
            var request = new RewindOffsetsToDateRequest { Date = _fixture.Create<DateTime>() };

            var consumers = new List<IMessageConsumer>
            {
                Mock.Of<IMessageConsumer>(c => c.GroupId == groupId && c.ConsumerName == consumerName),
            };

            _mockConsumerAccessor.Setup(x => x.All).Returns(consumers);

            // Act
            var result = await _target.RewindOffsets(groupId, consumerName, topics, request) as AcceptedResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(202);

            _mockConsumerAdmin.Verify(x => x.RewindOffsetsAsync(consumerName, request.Date, topics), Times.Once);
        }

        [TestMethod]
        public async Task RewindOffsets_InvalidRequest_ReturnsBadRequestResult()
        {
            // Arrange
            var groupId = "group1";
            var consumerName = "consumer1";
            var topics = new List<string> { "topic1", "topic2" };
            RewindOffsetsToDateRequest request = null;

            // Act
            var result = await _target.RewindOffsets(groupId, consumerName, topics, request) as BadRequestResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(400);

            _mockConsumerAdmin.Verify(x => x.RewindOffsetsAsync(consumerName, It.IsAny<DateTime>(), topics), Times.Never);
        }

        [TestMethod]
        public async Task ChangeWorkersCount_ValidRequest_ReturnsAcceptedResult()
        {
            // Arrange
            var groupId = "group1";
            var consumerName = "consumer1";
            var topics = new List<string> { "topic1", "topic2" };
            var request = new RewindOffsetsToDateRequest { Date = _fixture.Create<DateTime>() };

            var consumers = new List<IMessageConsumer>
            {
                Mock.Of<IMessageConsumer>(c => c.GroupId == groupId && c.ConsumerName == consumerName),
            };

            _mockConsumerAccessor.Setup(x => x.All).Returns(consumers);

            // Act
            var result = await _target.RewindOffsets(groupId, consumerName, topics, request) as AcceptedResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(202);

            _mockConsumerAdmin.Verify(x => x.RewindOffsetsAsync(consumerName, request.Date, topics), Times.Once);
        }

        [TestMethod]
        public async Task ChangeWorkersCount_NullRequest_ReturnsBadRequestResult()
        {
            // Arrange
            var groupId = "group1";
            var consumerName = "consumer1";
            ChangeWorkersCountRequest request = null;

            // Act
            var result = await _target.ChangeWorkersCount(groupId, consumerName, request) as BadRequestResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(400);

            _mockConsumerAdmin.Verify(x => x.ChangeWorkersCountAsync(consumerName, It.IsAny<int>()), Times.Never);
        }

        [DataRow(0)]
        [DataRow(-5)]
        [DataRow(-100)]
        [TestMethod]
        public async Task ChangeWorkersCount_InvalidWorkerCount_ReturnsBadRequestResult(int workerCount)
        {
            // Arrange
            var groupId = "group1";
            var consumerName = "consumer1";
            ChangeWorkersCountRequest request = new ChangeWorkersCountRequest { WorkersCount = workerCount };

            // Act
            var result = await _target.ChangeWorkersCount(groupId, consumerName, request) as BadRequestResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(400);

            _mockConsumerAdmin.Verify(x => x.ChangeWorkersCountAsync(consumerName, It.IsAny<int>()), Times.Never);
        }
    }
}
