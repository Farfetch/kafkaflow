namespace KafkaFlow.Consumers
{
    using System.Threading.Tasks;

    internal interface IWorkerPoolFeeder
    {
        void Start();

        Task StopAsync();
    }
}
