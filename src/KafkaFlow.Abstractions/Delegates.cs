namespace KafkaFlow
{
    using System.Threading.Tasks;

    public delegate Task MiddlewareDelegate(IMessageContext context);

    public delegate T Factory<out T>(IDependencyResolver resolver);
}
