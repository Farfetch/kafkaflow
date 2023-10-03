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
        private readonly Event target;
        private readonly Event<string> typedTarget;

        public EventTests()
        {
            var log = new Mock<ILogHandler>();
            this.target = new Event(log.Object);
            this.typedTarget = new Event<string>(log.Object);
        }

        [TestMethod]
        public async Task FireAsync_EventSubscribed_CallDelegateWithSuccess()
        {
            // Arrange
            var numberOfCalls = 0;

            this.target.Subscribe(() =>
            {
                numberOfCalls++;
                return Task.CompletedTask;
            });

            // Act
            await this.target.FireAsync();

            // Assert
            Assert.AreEqual(1, numberOfCalls);
        }

        [TestMethod]
        public async Task FireAsync_EventWithMultipleObservers_CallAllDelegatesWithSuccess()
        {
            // Arrange
            var numberOfCalls = 0;

            this.target.Subscribe(() =>
            {
                numberOfCalls++;
                return Task.CompletedTask;
            });

            this.target.Subscribe(() =>
            {
                numberOfCalls++;
                return Task.CompletedTask;
            });

            // Act
            await this.target.FireAsync();

            // Assert
            Assert.AreEqual(2, numberOfCalls);
        }

        [TestMethod]
        public async Task FireAsync_EventWithMultipleObserversAndErrors_CallAllDelegatesAndContinueWithoutErrors()
        {
            // Arrange
            var numberOfCalls = 0;

            this.target.Subscribe(() => throw new NotImplementedException());

            this.target.Subscribe(() =>
            {
                numberOfCalls++;
                return Task.CompletedTask;
            });

            // Act
            await this.target.FireAsync();

            // Assert
            Assert.AreEqual(1, numberOfCalls);
        }

        [TestMethod]
        public async Task FireAsync_EventSubscribedWithArgument_CallDelegateWithSuccess()
        {
            // Arrange
            var expectedArgument = Guid.NewGuid().ToString();
            var receivedArgument = string.Empty;

            this.typedTarget.Subscribe(arg =>
            {
                receivedArgument = arg;
                return Task.CompletedTask;
            });

            // Act
            await this.typedTarget.FireAsync(expectedArgument);

            // Assert
            Assert.AreEqual(expectedArgument, receivedArgument);
        }

        [TestMethod]
        public async Task FireAsync_EventWithMultipleObserversAndArgument_CallAllDelegatesWithSuccess()
        {
            // Arrange
            var expectedArgument = Guid.NewGuid().ToString();
            var receivedArguments = new List<string>();

            this.typedTarget.Subscribe(arg =>
            {
                receivedArguments.Add(arg);
                return Task.CompletedTask;
            });

            this.typedTarget.Subscribe(arg =>
            {
                receivedArguments.Add(arg);
                return Task.CompletedTask;
            });

            // Act
            await this.typedTarget.FireAsync(expectedArgument);

            // Assert
            Assert.AreEqual(2, receivedArguments.Count);
            Assert.IsTrue(receivedArguments.All(x => x == expectedArgument));
        }

        [TestMethod]
        public async Task FireAsync_TypedEventWithMultipleObserversAndErrors_CallAllDelegatesAndContinueWithoutErrors()
        {
            // Arrange
            var numberOfCalls = 0;

            this.typedTarget.Subscribe(_ => throw new NotImplementedException());

            this.typedTarget.Subscribe(_ =>
            {
                numberOfCalls++;
                return Task.CompletedTask;
            });

            // Act
            await this.typedTarget.FireAsync(Guid.NewGuid().ToString());

            // Assert
            Assert.AreEqual(1, numberOfCalls);
        }

        [TestMethod]
        public async Task FireAsync_DuplicatedEventHandler_CallHandlerOnce()
        {
            // Arrange
            var expectedArgument = Guid.NewGuid().ToString();
            var receivedArguments = new List<string>();

            Func<string, Task> handler = (arg) =>
            {
                receivedArguments.Add(arg);
                return Task.CompletedTask;
            };

            this.typedTarget.Subscribe(handler);
            this.typedTarget.Subscribe(handler);

            // Act
            await this.typedTarget.FireAsync(expectedArgument);

            // Assert
            Assert.AreEqual(1, receivedArguments.Count);
            Assert.IsTrue(receivedArguments.All(x => x == expectedArgument));
        }

        [TestMethod]
        public async Task FireAsync_UnsubscribeEventHandler_DoesNotCallHandler()
        {
            // Arrange
            var expectedArgument = Guid.NewGuid().ToString();
            var receivedArguments = new List<string>();

            Func<string, Task> handler = (arg) =>
            {
                receivedArguments.Add(arg);
                return Task.CompletedTask;
            };

            var subscription = this.typedTarget.Subscribe(handler);

            subscription.Cancel();

            // Act
            await this.typedTarget.FireAsync(expectedArgument);

            // Assert
            Assert.AreEqual(0, receivedArguments.Count);
        }
    }
}
