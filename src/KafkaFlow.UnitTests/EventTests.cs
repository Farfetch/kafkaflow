namespace KafkaFlow.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class EventTests
    {
        [TestMethod]
        public async Task FireAsync_EventSubscribed_CallDelegateWithSuccess()
        {
            // Arrange
            var numberOfCalls = 0;
            var log = new Mock<ILogHandler>();

            var event1 = new Event(log.Object);

            event1.Subscribe(() =>
            {
                numberOfCalls++;
                return Task.CompletedTask;
            });

            // Act
            await event1.FireAsync();

            // Assert
            Assert.AreEqual(1, numberOfCalls);
        }

        [TestMethod]
        public async Task FireAsync_EventWithMultipleObservers_CallAllDelegatesWithSuccess()
        {
            // Arrange
            var numberOfCalls = 0;
            var log = new Mock<ILogHandler>();

            var event1 = new Event(log.Object);

            event1.Subscribe(() =>
            {
                numberOfCalls++;
                return Task.CompletedTask;
            });

            event1.Subscribe(() =>
            {
                numberOfCalls++;
                return Task.CompletedTask;
            });

            // Act
            await event1.FireAsync();

            // Assert
            Assert.AreEqual(2, numberOfCalls);
        }

        [TestMethod]
        public async Task FireAsync_EventWithMultipleObserversAndErrors_CallAllDelegatesAndContinueWithoutErrors()
        {
            // Arrange
            var numberOfCalls = 0;
            var log = new Mock<ILogHandler>();

            var event1 = new Event(log.Object);

            event1.Subscribe(() =>
            {
                throw new System.Exception();
            });

            event1.Subscribe(() =>
            {
                numberOfCalls++;
                return Task.CompletedTask;
            });

            // Act
            await event1.FireAsync();

            // Assert
            Assert.AreEqual(1, numberOfCalls);
        }

        [TestMethod]
        public async Task FireAsync_EventSubscribedWithArgument_CallDelegateWithSuccess()
        {
            // Arrange
            var expectedArgument = Guid.NewGuid().ToString();
            var receivedArgument = string.Empty;
            var log = new Mock<ILogHandler>();

            var event1 = new Event<string>(log.Object);

            event1.Subscribe(arg =>
            {
                receivedArgument = arg;
                return Task.CompletedTask;
            });

            // Act
            await event1.FireAsync(expectedArgument);

            // Assert
            Assert.AreEqual(expectedArgument, receivedArgument);
        }

        [TestMethod]
        public async Task FireAsync_EventWithMultipleObserversAndArgument_CallAllDelegatesWithSuccess()
        {
            // Arrange
            var expectedArgument = Guid.NewGuid().ToString();
            var receivedArguments = new List<string>();

            var log = new Mock<ILogHandler>();

            var event1 = new Event<string>(log.Object);

            event1.Subscribe(arg =>
            {
                receivedArguments.Add(arg);
                return Task.CompletedTask;
            });

            event1.Subscribe(arg =>
            {
                receivedArguments.Add(arg);
                return Task.CompletedTask;
            });

            // Act
            await event1.FireAsync(expectedArgument);

            // Assert
            Assert.AreEqual(2, receivedArguments.Count);
            Assert.IsTrue(receivedArguments.All(x => x == expectedArgument));
        }

        [TestMethod]
        public async Task FireAsync_TypedEventWithMultipleObserversAndErrors_CallAllDelegatesAndContinueWithoutErrors()
        {
            // Arrange
            var numberOfCalls = 0;
            var log = new Mock<ILogHandler>();

            var event1 = new Event<string>(log.Object);

            event1.Subscribe(_ =>
            {
                throw new System.Exception();
            });

            event1.Subscribe(_ =>
            {
                numberOfCalls++;
                return Task.CompletedTask;
            });

            // Act
            await event1.FireAsync(Guid.NewGuid().ToString());

            // Assert
            Assert.AreEqual(1, numberOfCalls);
        }
    }
}
