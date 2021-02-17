namespace KafkaFlow.BatchConsume
{
    using System.Threading.Tasks;

    internal interface IWorkerBatch
    {
        Task AddAsync(IMessageContext context, MiddlewareDelegate next);
    }
}
