using System;
using System.Collections.Generic;
using System.Linq;
using Confluent.Kafka;
using KafkaFlow.Consumers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace KafkaFlow.UnitTests.Consumer;

[TestClass]
public sealed class ConsumerContextPauseResumeTests
{
    private Mock<IConsumer> _consumerMock;
    private Mock<IConsumerFlowManager> _flowManagerMock;
    private IConsumerContext _consumerContext;

    private static readonly List<Confluent.Kafka.TopicPartition> s_topicPartitions =
    [
        new("topic-1", 1),
        new("topic-1", 2),
        new("topic-2", 2)
    ];

    [TestInitialize]
    public void Setup()
    {
        _consumerMock = new Mock<IConsumer>();
        _flowManagerMock = new Mock<IConsumerFlowManager>();

        _consumerMock.Setup(x => x.FlowManager).Returns(_flowManagerMock.Object);
        _consumerMock.Setup(x => x.Configuration.AutoMessageCompletion).Returns(true);

        var consumeResult = new ConsumeResult<byte[], byte[]>
        {
            Message = new Message<byte[], byte[]>()
        };

        var worker = new Mock<IConsumerWorker>();
        _consumerContext = new ConsumerContext(
            consumer: _consumerMock.Object,
            offsetManager: Mock.Of<IOffsetManager>(),
            kafkaResult: consumeResult,
            worker: worker.Object,
            messageDependencyScope: Mock.Of<IDependencyResolverScope>(),
            consumerDependencyResolver: Mock.Of<IDependencyResolver>());

        _consumerMock.Setup(x => x.FlowManager).Returns(_flowManagerMock.Object);
        _consumerMock.Setup(x => x.Configuration.AutoMessageCompletion).Returns(true);
    }

    [TestMethod]
    public void Pause_WithNoAssignment_PausesEmptyListOfPartitions()
    {
        // Arrange
        _consumerMock.SetupGet(x => x.Assignment).Returns([]);

        // Act
        _consumerContext.Pause(s_topicPartitions.Select(x => x.ToKafkaFlow()).ToArray());

        // Assert
        IReadOnlyList<Confluent.Kafka.TopicPartition> expectedPartitions = [];
        _flowManagerMock.Verify(x => x.Pause(expectedPartitions));
    }

    [TestMethod]
    public void Resume_WithNoAssignment_ResumesEmptyListOfPartitions()
    {
        // Arrange
        _consumerMock.SetupGet(x => x.Assignment).Returns([]);

        // Act
        _consumerContext.Resume(s_topicPartitions.Select(x => x.ToKafkaFlow()).ToArray());

        // Assert
        IReadOnlyList<Confluent.Kafka.TopicPartition> expectedPartitions = [];
        _flowManagerMock.Verify(x => x.Resume(expectedPartitions));
    }

    [TestMethod]
    public void Pause_WithAssignment_PausesSpecificPartitions()
    {
        // Arrange
        _consumerMock.SetupGet(x => x.Assignment).Returns(s_topicPartitions);
        var (toPause, _) = s_topicPartitions.RandomSplit();

        // Add an extra partition not assigned to the consumer
        var actOnPartitions = toPause
            .Select(x => x.ToKafkaFlow())
            .Append(new TopicPartition("topic-3", 3))
            .ToArray();

        // Act
        _consumerContext.Pause(actOnPartitions);

        // Assert
        _flowManagerMock.Verify(x => x.Pause(toPause.ToVerification()));
    }

    [TestMethod]
    public void Resume_WithAssignment_ResumesSpecificPartitions()
    {
        // Arrange
        _consumerMock.SetupGet(x => x.Assignment).Returns(s_topicPartitions);
        var (_, toResume) = s_topicPartitions.RandomSplit();

        // Add an extra partition not assigned to the consumer
        var actOnPartitions = toResume
            .Select(x => x.ToKafkaFlow())
            .Append(new TopicPartition("topic-3", 3))
            .ToArray();

        // Act
        _consumerContext.Resume(actOnPartitions);

        // Assert
        _flowManagerMock.Verify(x => x.Resume(toResume.ToVerification()));
    }
}

file static class TestExtensions
{
    public static IReadOnlyList<T> ToVerification<T>(this IReadOnlyList<T> values) =>
        It.Is<IReadOnlyList<T>>(x => x.Count == values.Count && x.All(values.Contains));

    public static TopicPartition ToKafkaFlow(this Confluent.Kafka.TopicPartition topicPartition) =>
        new(topicPartition.Topic, topicPartition.Partition);

    /// <summary>
    /// Splits the list into two random parts.
    /// </summary>
    public static (T[], T[]) RandomSplit<T>(this IList<T> values)
    {
        switch (values.Count)
        {
            case 0:
                return ([], []);
            case 1:
                return ([..values], []);
            default:
                var splitIndex = Random.Shared.Next(1, values.Count - 1);
                var shuffledArray = values.OrderBy(_ => Random.Shared.Next()).ToArray();
                return ([..shuffledArray[..splitIndex]], [..shuffledArray[splitIndex..]]);
        }
    }
}
