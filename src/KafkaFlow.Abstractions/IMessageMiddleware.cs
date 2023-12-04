using System.Threading.Tasks;

namespace KafkaFlow;

/// <summary>
/// Used to create a message middleware
/// </summary>
public interface IMessageMiddleware
{
    /// <summary>
    /// The method that is called when the middleware is invoked
    /// </summary>
    /// <param name="context">The message context</param>
    /// <param name="next">A delegate to the next middleware</param>
    /// <returns></returns>
    Task Invoke(IMessageContext context, MiddlewareDelegate next);
}
