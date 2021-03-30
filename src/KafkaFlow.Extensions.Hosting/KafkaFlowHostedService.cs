namespace KafkaFlow
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using global::Microsoft.Extensions.Hosting;

    internal class KafkaFlowHostedService : IHostedService
    {
        private readonly IKafkaBus kafkaBus;

        public KafkaFlowHostedService(IServiceProvider serviceProvider) => this.kafkaBus = serviceProvider.CreateKafkaBus();

        public Task StartAsync(CancellationToken cancellationToken) => this.kafkaBus.StartAsync(cancellationToken);

        public Task StopAsync(CancellationToken cancellationToken) => this.kafkaBus.StopAsync();
    }
}
