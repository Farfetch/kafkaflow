namespace KafkaFlow.Client
{
    using System;

    public class ProduceException : Exception
    {
        public ProduceException(Exception inner) : base("Message produce failed", inner)
        {
        }
    }
}
