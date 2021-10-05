namespace KafkaFlow.Extensions.ProducerThrottling.Evaluations
{
    using System.Linq;
    using System.Threading.Tasks;
    using KafkaFlow.Client.Metrics;

    /// <summary>
    /// Evaluate the lag metric value of a consumer group in a topic
    /// </summary>
    public class LagEvaluation : IEvaluation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LagEvaluation"/> class.
        /// </summary>
        /// <param name="resolver">Instance of <see cref="IDependencyResolver"/></param>
        /// <param name="topic">The topic name</param>
        /// <param name="consumerGroup">The consumer group</param>
        public LagEvaluation(IDependencyResolver resolver, string topic, string consumerGroup)
        {
            this.Topic = topic;
            this.ConsumerGroup = consumerGroup;
            this.LagReader = resolver.Resolve<ILagReader>();
        }

        private ILagReader LagReader { get; }

        private string ConsumerGroup { get; }

        private string Topic { get; }

        /// <inheritdoc />
        public async Task<long> Eval()
        {
            var lags = await this.LagReader.GetLagAsync(this.Topic, this.ConsumerGroup);
            return lags.Sum(l => l.Lag);
        }
    }
}
