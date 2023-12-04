using System;
using System.Threading.Tasks;

namespace KafkaFlow;

internal interface IMiddlewareExecutor
{
    Task Execute(IMessageContext context, Func<IMessageContext, Task> nextOperation);
}
