namespace KafkaFlow.Client.Producers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using KafkaFlow.Client.Metrics;
    using KafkaFlow.Client.Protocol;

    public class MetricsReaderBuilder
    {
        /// <summary>
        /// A builder to create metrics reader
        /// </summary>
        /// <param name="brokers">The list of brokers</param>
        /// <param name="clientId">Identifier of the client</param>
        /// <param name="timeout">The max time span to wait for the request to be answered, otherwise a <see cref="TimeoutException"/> will be thrown.</param>
        /// <returns></returns>
        public static ILagReader BuildLagReader(
            IEnumerable<BrokerAddress> brokers,
            string? clientId = null,
            TimeSpan? timeout = null)
        {
            return new LagReader(
                new KafkaCluster(
                    brokers.ToList(),
                    clientId ?? Guid.NewGuid().ToString(),
                    timeout ?? TimeSpan.FromSeconds(5)));
        }
    }
}
