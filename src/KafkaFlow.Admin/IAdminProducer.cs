namespace KafkaFlow.Admin
{
    using System.Threading.Tasks;
    using KafkaFlow.Admin.Messages;

    public interface IAdminProducer
    {
        Task ProduceAsync(IAdminMessage message);
    }
}
