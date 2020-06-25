namespace KafkaFlow
{
    using System;

    public interface IDependencyResolver
    {
        IDependencyResolverScope CreateScope();

        object Resolve(Type type);
    }
}
