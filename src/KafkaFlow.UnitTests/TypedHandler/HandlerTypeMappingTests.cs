namespace KafkaFlow.UnitTests.TypedHandler
{
    using System;
    using FluentAssertions;
    using KafkaFlow.TypedHandler;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class HandlerTypeMappingTests
    {
        private HandlerTypeMapping target;
        
        [TestInitialize]
        public void Setup()
        {
            this.target = new HandlerTypeMapping();
        }

        [TestMethod]
        public void AddSeveralMappings_GetHandlersTypesReturnsListOfHandlers()
        {
            // Act
            this.target.AddMapping(typeof(int), typeof(string));
            this.target.AddMapping(typeof(int), typeof(double));
            this.target.AddMapping(typeof(int), typeof(bool));

            // Assert
            this.target.GetHandlersTypes(typeof(int)).Should().BeEquivalentTo(
                typeof(string), 
                typeof(double),
                typeof(bool));
        }
    }
}