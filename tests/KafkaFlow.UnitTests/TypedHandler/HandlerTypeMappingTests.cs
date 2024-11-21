using System;
using System.Collections.Generic;
using FluentAssertions;
using KafkaFlow.Middlewares.TypedHandler;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KafkaFlow.UnitTests.TypedHandler;

[TestClass]
public class HandlerTypeMappingTests
{
    private HandlerTypeMapping _target;

    [TestInitialize]
    public void Setup()
    {
        _target = new HandlerTypeMapping();
    }

    [TestMethod]
    public void AddSeveralMappings_GetHandlersTypesReturnsListOfHandlers()
    {
        // Act
        _target.AddMapping(typeof(int), typeof(string));
        _target.AddMapping(typeof(int), typeof(double));
        _target.AddMapping(typeof(int), typeof(bool));

        // Assert
        _target.GetHandlersTypes(typeof(int))
            .Should()
            .BeEquivalentTo(new List<Type>
            {
                typeof(string),
                typeof(double),
                typeof(bool)
            });
    }
}
