using System.Threading.Tasks;

namespace KafkaFlow.Consumers
{
    internal interface IWorkerPoolFeeder
    {
        void Start();

        Task StopAsync();
    }
}
