using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace KafkaFlow.UnitTests;

[TestClass]
public class EventTests
{
    private readonly Event _target;
    private readonly Event<string> _typedTarget;

    public EventTests()
    {
        var log = new Mock<ILogHandler>();
        _target = new Event(log.Object);
        _typedTarget = new Event<string>(log.Object);
    }

    [TestMethod]
    public async Task FireAsync_EventSubscribed_CallDelegateWithSuccess()
    {
        // Arrange
        var numberOfCalls = 0;

        _target.Subscribe(() =>
        {
            numberOfCalls++;
            return Task.CompletedTask;
        });

        // Act
        await _target.FireAsync();

        // Assert
        Assert.AreEqual(1, numberOfCalls);
    }

    [TestMethod]
    public async Task FireAsync_EventWithMultipleObservers_CallAllDelegatesWithSuccess()
    {
        // Arrange
        var numberOfCalls = 0;

        _target.Subscribe(() =>
        {
            numberOfCalls++;
            return Task.CompletedTask;
        });

        _target.Subscribe(() =>
        {
            numberOfCalls++;
            return Task.CompletedTask;
        });

        // Act
        await _target.FireAsync();

        // Assert
        Assert.AreEqual(2, numberOfCalls);
    }

    [TestMethod]
    public async Task FireAsync_EventWithMultipleObserversAndErrors_CallAllDelegatesAndContinueWithoutErrors()
    {
        // Arrange
        var numberOfCalls = 0;

        _target.Subscribe(() => throw new NotImplementedException());

        _target.Subscribe(() =>
        {
            numberOfCalls++;
            return Task.CompletedTask;
        });

        // Act
        await _target.FireAsync();

        // Assert
        Assert.AreEqual(1, numberOfCalls);
    }

    [TestMethod]
    public async Task FireAsync_EventSubscribedWithArgument_CallDelegateWithSuccess()
    {
        // Arrange
        var expectedArgument = Guid.NewGuid().ToString();
        var receivedArgument = string.Empty;

        _typedTarget.Subscribe(arg =>
        {
            receivedArgument = arg;
            return Task.CompletedTask;
        });

        // Act
        await _typedTarget.FireAsync(expectedArgument);

        // Assert
        Assert.AreEqual(expectedArgument, receivedArgument);
    }

    [TestMethod]
    public async Task FireAsync_EventWithMultipleObserversAndArgument_CallAllDelegatesWithSuccess()
    {
        // Arrange
        var expectedArgument = Guid.NewGuid().ToString();
        var receivedArguments = new List<string>();

        _typedTarget.Subscribe(arg =>
        {
            receivedArguments.Add(arg);
            return Task.CompletedTask;
        });

        _typedTarget.Subscribe(arg =>
        {
            receivedArguments.Add(arg);
            return Task.CompletedTask;
        });

        // Act
        await _typedTarget.FireAsync(expectedArgument);

        // Assert
        Assert.AreEqual(2, receivedArguments.Count);
        Assert.IsTrue(receivedArguments.All(x => x == expectedArgument));
    }

    [TestMethod]
    public async Task FireAsync_TypedEventWithMultipleObserversAndErrors_CallAllDelegatesAndContinueWithoutErrors()
    {
        // Arrange
        var numberOfCalls = 0;

        _typedTarget.Subscribe(_ => throw new NotImplementedException());

        _typedTarget.Subscribe(_ =>
        {
            numberOfCalls++;
            return Task.CompletedTask;
        });

        // Act
        await _typedTarget.FireAsync(Guid.NewGuid().ToString());

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

        _typedTarget.Subscribe(handler);
        _typedTarget.Subscribe(handler);

        // Act
        await _typedTarget.FireAsync(expectedArgument);

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

        var subscription = _typedTarget.Subscribe(handler);

        subscription.Cancel();

        // Act
        await _typedTarget.FireAsync(expectedArgument);

        // Assert
        Assert.AreEqual(0, receivedArguments.Count);
    }
}
