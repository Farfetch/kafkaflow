namespace KafkaFlow.Client
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    internal class KafkaConnectionManager
    {
        private readonly IReadOnlyCollection<KafkaHostAddress> hostAddresses;

        public KafkaConnectionManager(IReadOnlyCollection<KafkaHostAddress> hostAddresses)
        {
            this.hostAddresses = hostAddresses;
        }

        // public Task ConnectAsync(CancellationToken cancelToken = default)
        // {
        // }
    }
}
