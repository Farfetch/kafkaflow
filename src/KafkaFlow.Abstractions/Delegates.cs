namespace KafkaFlow
{
    using System.Threading.Tasks;

    /// <summary>
    /// Defines a middleware operation receiving a <see cref="IMessageContext"/> parameter
    /// </summary>
    /// <param name="context"></param>
    public delegate Task MiddlewareDelegate(IMessageContext context);

    /// <summary>
    /// Defines a factory to create an instance of <typeparamref name="T" /> type 
    /// </summary>
    /// <param name="resolver">A class that implements <see cref="IDependencyResolver"/></param>
    public delegate T Factory<out T>(IDependencyResolver resolver);
}
