namespace KafkaFlow.Configuration
{
    using System.Collections.Generic;

    internal class MiddlewareConfiguration
    {
        public IReadOnlyList<Factory<IMessageMiddleware>> Factories { get; }

        public bool CloneContext { get; }

        public MiddlewareConfiguration(IReadOnlyList<Factory<IMessageMiddleware>> factories, bool cloneContext)
        {
            this.Factories = factories;
            this.CloneContext = cloneContext;
        }
    }
}
