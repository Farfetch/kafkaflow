namespace KafkaFlow.Configuration
{
    using System.Collections.Generic;

    internal class MiddlewareConfiguration
    {
        public IReadOnlyList<Factory<IMessageMiddleware>> Factories { get; }

        public MiddlewareConfiguration(IReadOnlyList<Factory<IMessageMiddleware>> factories)
        {
            this.Factories = factories;
        }
    }
}
