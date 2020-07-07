namespace KafkaFlow.IntegrationTests
{
    using System.Threading.Tasks;
    using global::Microsoft.VisualStudio.TestTools.UnitTesting;
    using KafkaFlow.IntegrationTests.Common;
    using KafkaFlow.IntegrationTests.Common.Core.Producers;
    using KafkaFlow.Producers;

    [TestClass]
    public class CompressionSerializationTest : BaseCompressionSerializationTest
    {
        [TestInitialize]
        public void Setup()
        {
            base.Setup();
        }

        [TestMethod]
        public Task JsonGzipMessageTest()
        {
            return base.JsonGzipMessageTest(Bootstrapper.GetService<IMessageProducer<JsonGzipProducer>>());
        }
        
        [TestMethod]
        public Task ProtoBufGzipMessageTest()
        {
            return base.ProtoBufGzipMessageTest(Bootstrapper.GetService<IMessageProducer<ProtobufGzipProducer>>());
        }
    }
}
