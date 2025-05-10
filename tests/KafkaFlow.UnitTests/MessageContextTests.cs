using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace KafkaFlow.UnitTests;

[TestClass]
public class MessageContextTests
{
    [TestMethod]
    public void SetMessage_ShouldSetMessageCorrectly()
    {
        // Arrange
        var messageContext = new MessageContext(
            new Message("key", "value"),
            Mock.Of<IMessageHeaders>(),
            Mock.Of<IDependencyResolver>(),
            Mock.Of<IConsumerContext>(),
            Mock.Of<IProducerContext>(),
            Mock.Of<IReadOnlyCollection<string>>()
        );


        // Act
        var changedMessage = messageContext.SetMessage("changed-key", "changed-value");

        // Assert
        Assert.AreEqual("changed-key", changedMessage.Message.Key);
        Assert.AreEqual("changed-value", changedMessage.Message.Value);
        Assert.AreSame(messageContext.ConsumerContext, changedMessage.ConsumerContext);
        Assert.AreSame(messageContext.DependencyResolver, changedMessage.DependencyResolver);
        Assert.AreSame(messageContext.Headers, changedMessage.Headers);
        Assert.AreSame(messageContext.ProducerContext, changedMessage.ProducerContext);
        Assert.AreSame(messageContext.Brokers, changedMessage.Brokers);
        Assert.AreSame(messageContext.Items, changedMessage.Items);
    }
}
