namespace KafkaFlow.Configuration
{
    using System.Collections.Generic;

    public class MiddlewareConfiguration
    {
        public MiddlewareConfiguration(IReadOnlyList<Factory<IMessageMiddleware>> factories)
        {
            this.Factories = factories;
        }

        public IReadOnlyList<Factory<IMessageMiddleware>> Factories { get; }
    }
}
