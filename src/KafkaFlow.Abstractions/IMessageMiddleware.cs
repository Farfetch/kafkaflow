namespace KafkaFlow
{
    using System.Threading.Tasks;

    /// <summary>
    /// Represents the interface to be implemented by custom message middlewares
    /// </summary>
    public interface IMessageMiddleware
    {
        /// <summary>
        /// Executes an operation using data from <see cref="IMessageContext"/>
        /// </summary>
        /// <param name="context">Instance of <see cref="IMessageContext"/></param>
        /// <param name="next">Next middleware to be executed</param>
        /// <returns></returns>
        Task Invoke(IMessageContext context, MiddlewareDelegate next);
    }
}
