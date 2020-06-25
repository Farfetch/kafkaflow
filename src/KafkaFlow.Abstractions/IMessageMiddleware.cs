namespace KafkaFlow
{
    using System.Threading.Tasks;

    public interface IMessageMiddleware
    {
        Task Invoke(IMessageContext context, MiddlewareDelegate next);
    }
}
