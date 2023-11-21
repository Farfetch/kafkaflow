using System;
using System.Threading;
using System.Threading.Tasks;
using global::Microsoft.Extensions.Hosting;

namespace KafkaFlow
{
    internal class KafkaFlowHostedService : IHostedService
    {
        private readonly IKafkaBus _kafkaBus;

        public KafkaFlowHostedService(IServiceProvider serviceProvider) => _kafkaBus = serviceProvider.CreateKafkaBus();

        public Task StartAsync(CancellationToken cancellationToken) => _kafkaBus.StartAsync(cancellationToken);

        public Task StopAsync(CancellationToken cancellationToken) => _kafkaBus.StopAsync();
    }
}
