namespace KafkaFlow.Producers.Middlewares.Throttling.Evaluations
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using KafkaFlow.Consumers;
    using KafkaFlow.Producers.Middlewares.Throttling.Actions;

    internal class LagEvaluation : IEvaluation
    {
        private readonly IClusterAccessor clusterAccessor;
        private readonly string topic;
        private readonly IReadOnlyCollection<string> consumerGroups;

        public LagEvaluation(IClusterAccessor clusterAccessor, string topic, IReadOnlyCollection<string> consumerGroups)
        {
            this.clusterAccessor = clusterAccessor;
            this.topic = topic;
            this.consumerGroups = consumerGroups;
        }

        public async Task<IAction> EvaluateAsync(IMessageContext context, IReadOnlyList<IAction> actions)
        {
            var cluster = this.clusterAccessor[context.ClusterName];

            var lags = await cluster.MetadataClient
                .GetTopicLag(this.topic, this.consumerGroups)
                .ConfigureAwait(false);

            var maxLag = lags.ConsumerGroups
                .Select(cg => cg.Partitions.Select(p => p.Lag).Sum())
                .Max();

            return actions.LastOrDefault(act => act.Threshold <= maxLag);
        }
    }
}
