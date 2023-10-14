using KafkaFlow.IntegrationTests.Core.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KafkaFlow.IntegrationTests.Core.Middlewares
{
    internal class TriggerErrorMessageMiddleware : IMessageMiddleware
    {
        public async Task Invoke(IMessageContext context, MiddlewareDelegate next)
        {
            MessageStorage.Add((byte[])context.Message.Value);
            throw new Exception();
            await next(context);
        }
    }
}
