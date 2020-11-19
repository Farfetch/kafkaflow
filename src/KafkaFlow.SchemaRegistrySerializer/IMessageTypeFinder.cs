namespace KafkaFlow.SchemaRegistrySerializer
{
    using System;

    internal interface IMessageTypeFinder
    {
        Type Find(string subject, int version);
    }
}
