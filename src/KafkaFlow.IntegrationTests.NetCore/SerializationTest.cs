namespace KafkaFlow.IntegrationTests
{
    using System.Threading.Tasks;
    using global::Microsoft.VisualStudio.TestTools.UnitTesting;
    using KafkaFlow.IntegrationTests.Common.Core.Producers;
    using Producers;

    [TestClass]
    public class SerializationTest : BaseSerializationTest
    {
        [TestInitialize]
        public void Setup()
        {
            base.Setup();
        }

        [TestMethod]
        public Task JsonMessageTest()
        {
            return base.JsonMessageTest(Bootstrapper.GetService<IMessageProducer<JsonProducer>>());
        }
        
        [TestMethod]
        public Task ProtobufMessageTest()
        {
            return base.ProtobufMessageTest(Bootstrapper.GetService<IMessageProducer<ProtobufProducer>>());
        }
    }
}
