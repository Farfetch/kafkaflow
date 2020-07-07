namespace KafkaFlow.IntegrationTests
{
    using System.Threading.Tasks;
    using global::Microsoft.VisualStudio.TestTools.UnitTesting;
    using KafkaFlow.IntegrationTests.Common.Core.Producers;
    using KafkaFlow.Producers;

    [TestClass]
    public class ConsumerTest : BaseConsumerTest
    {
        [TestInitialize]
        public void Setup()
        {
            base.Setup();
        }

        [TestMethod]
        public Task MultipleMessagesMultipleHandlersSingleTopicTest()
        {

            return base.MultipleMessagesMultipleHandlersSingleTopicTest(Bootstrapper.GetService<IMessageProducer<JsonProducer>>());
        }

        [TestMethod]
        public Task MultipleTopicsSingleConsumerTest()
        {
            return base.MultipleTopicsSingleConsumerTest(
                Bootstrapper.GetService<IMessageProducer<ProtobufGzipProducer>>(),
                Bootstrapper.GetService<IMessageProducer<ProtobufGzipProducer2>>());
        }

        [TestMethod]
        public Task MessageOrderingTest()
        {
            return base.MessageOrderingTest(Bootstrapper.GetService<IMessageProducer<ProtobufProducer>>());
        }
    }
}
