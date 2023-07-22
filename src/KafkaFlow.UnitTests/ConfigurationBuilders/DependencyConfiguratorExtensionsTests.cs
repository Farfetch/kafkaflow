namespace KafkaFlow.UnitTests.ConfigurationBuilders
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class DependencyConfiguratorExtensionsTests
    {
        private static IEnumerable<object[]> MethodsForAdd
        {
            get
            {
                yield return new object[]
                {
                    (Action<IDependencyConfigurator>)(configurator => configurator.AddSingleton<IList, ArrayList>()),
                    (Expression<Action<IDependencyConfigurator>>)(configurator => configurator.Add<IList, ArrayList>(InstanceLifetime.Singleton)),
                };
                yield return new object[]
                {
                    (Action<IDependencyConfigurator>)(configurator => configurator.AddSingleton<ArrayList>()),
                    (Expression<Action<IDependencyConfigurator>>)(configurator => configurator.Add<ArrayList>(InstanceLifetime.Singleton)),
                };
                yield return new object[]
                {
                    (Action<IDependencyConfigurator>)(configurator => configurator.AddSingleton(new ArrayList())),
                    (Expression<Action<IDependencyConfigurator>>)(configurator => configurator.Add(It.IsAny<ArrayList>())),
                };
                yield return new object[]
                {
                    (Action<IDependencyConfigurator>)(configurator => configurator.AddSingleton(_ => new ArrayList())),
                    (Expression<Action<IDependencyConfigurator>>)(configurator => configurator.Add(typeof(ArrayList), It.IsAny<Func<IDependencyResolver, ArrayList>>(), InstanceLifetime.Singleton)),
                };
                yield return new object[]
                {
                    (Action<IDependencyConfigurator>)(configurator => configurator.AddTransient<IList, ArrayList>()),
                    (Expression<Action<IDependencyConfigurator>>)(configurator => configurator.Add<IList, ArrayList>(InstanceLifetime.Transient)),
                };
                yield return new object[]
                {
                    (Action<IDependencyConfigurator>)(configurator => configurator.AddTransient<ArrayList>()),
                    (Expression<Action<IDependencyConfigurator>>)(configurator => configurator.Add<ArrayList>(InstanceLifetime.Transient)),
                };
                yield return new object[]
                {
                    (Action<IDependencyConfigurator>)(configurator => configurator.AddTransient(_ => new ArrayList())),
                    (Expression<Action<IDependencyConfigurator>>)(configurator => configurator.Add(typeof(ArrayList), It.IsAny<Func<IDependencyResolver, ArrayList>>(), InstanceLifetime.Transient)),
                };
            }
        }

        private static IEnumerable<object[]> MethodsForTryAdd
        {
            get
            {
                yield return new object[]
                {
                    (Action<IDependencyConfigurator>)(configurator => configurator.TryAddSingleton<IList, ArrayList>()),
                    (Expression<Action<IDependencyConfigurator>>)(configurator => configurator.Add<IList, ArrayList>(InstanceLifetime.Singleton)),
                };
                yield return new object[]
                {
                    (Action<IDependencyConfigurator>)(configurator => configurator.TryAddSingleton<ArrayList>()),
                    (Expression<Action<IDependencyConfigurator>>)(configurator => configurator.Add<ArrayList>(InstanceLifetime.Singleton)),
                };
                yield return new object[]
                {
                    (Action<IDependencyConfigurator>)(configurator => configurator.TryAddSingleton(new ArrayList())),
                    (Expression<Action<IDependencyConfigurator>>)(configurator => configurator.Add(It.IsAny<ArrayList>())),
                };
                yield return new object[]
                {
                    (Action<IDependencyConfigurator>)(configurator => configurator.TryAddSingleton(_ => new ArrayList())),
                    (Expression<Action<IDependencyConfigurator>>)(configurator => configurator.Add(typeof(ArrayList), It.IsAny<Func<IDependencyResolver, ArrayList>>(), InstanceLifetime.Singleton)),
                };
                yield return new object[]
                {
                    (Action<IDependencyConfigurator>)(configurator => configurator.TryAddTransient<IList, ArrayList>()),
                    (Expression<Action<IDependencyConfigurator>>)(configurator => configurator.Add<IList, ArrayList>(InstanceLifetime.Transient)),
                };
                yield return new object[]
                {
                    (Action<IDependencyConfigurator>)(configurator => configurator.TryAddTransient<ArrayList>()),
                    (Expression<Action<IDependencyConfigurator>>)(configurator => configurator.Add<ArrayList>(InstanceLifetime.Transient)),
                };
                yield return new object[]
                {
                    (Action<IDependencyConfigurator>)(configurator => configurator.TryAddTransient(_ => new ArrayList())),
                    (Expression<Action<IDependencyConfigurator>>)(configurator => configurator.Add(typeof(ArrayList), It.IsAny<Func<IDependencyResolver, ArrayList>>(), InstanceLifetime.Transient)),
                };
            }
        }

        [TestMethod]
        [DynamicData(nameof(MethodsForAdd))]
        public void Add(Action<IDependencyConfigurator> addMethod, Expression<Action<IDependencyConfigurator>> underlyingAddMethod)
        {
            // Arrange
            var configurator = new Mock<IDependencyConfigurator>();

            // Act
            addMethod(configurator.Object);

            // Assert
            configurator.Verify(underlyingAddMethod, Times.Once);
        }

        [TestMethod]
        [DynamicData(nameof(MethodsForTryAdd))]
        public void TryAdd_AddWhenNotExists(Action<IDependencyConfigurator> tryAddMethod, Expression<Action<IDependencyConfigurator>> underlyingAddMethod)
        {
            // Arrange
            var configurator = new Mock<IDependencyConfigurator>();
            configurator.Setup(c => c.AlreadyRegistered(typeof(IList))).Returns(false);
            configurator.Setup(c => c.AlreadyRegistered(typeof(ArrayList))).Returns(false);

            // Act
            tryAddMethod(configurator.Object);

            // Assert
            configurator.Verify(underlyingAddMethod, Times.Once);
        }

        [TestMethod]
        [DynamicData(nameof(MethodsForTryAdd))]
        public void TryAdd_IgnoreWhenExists(Action<IDependencyConfigurator> tryAddMethod, Expression<Action<IDependencyConfigurator>> underlyingAddMethod)
        {
            // Arrange
            var configurator = new Mock<IDependencyConfigurator>();
            configurator.Setup(c => c.AlreadyRegistered(typeof(IList))).Returns(true);
            configurator.Setup(c => c.AlreadyRegistered(typeof(ArrayList))).Returns(true);

            // Act
            tryAddMethod(configurator.Object);

            // Assert
            configurator.Verify(underlyingAddMethod, Times.Never);
        }
    }
}
