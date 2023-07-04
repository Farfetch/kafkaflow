namespace KafkaFlow
{
    using System;
    using System.Threading.Tasks;

    internal interface IMiddlewareExecutor
    {
        Task Execute(IMessageContext context, Func<IMessageContext, Task> nextOperation);

        void ClearWorkersMiddlewaresInstances();
    }
}
