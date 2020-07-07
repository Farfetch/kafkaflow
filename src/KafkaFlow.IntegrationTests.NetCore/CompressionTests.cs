namespace KafkaFlow.IntegrationTests
{
    using System.Threading.Tasks;
    using global::Microsoft.VisualStudio.TestTools.UnitTesting;
    using KafkaFlow.IntegrationTests.Common;
    using KafkaFlow.IntegrationTests.Common.Core.Producers;
    using KafkaFlow.Producers;
    
    [TestClass]
    public class CompressionTests : BaseCompressionTests
    {
        [TestInitialize]
        public void Setup()
        {
            base.Setup();
        }

        [TestMethod]
        public Task GzipTest()
        {
            return base.GzipTest(Bootstrapper.GetService<IMessageProducer<GzipProducer>>());
        }
    }
}
