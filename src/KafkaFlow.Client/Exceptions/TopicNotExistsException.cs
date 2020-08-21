namespace KafkaFlow.Client.Exceptions
{
    using System;

    public class TopicNotExistsException : Exception
    {
        public string TopicName { get; }

        public TopicNotExistsException(string topicName)
        {
            this.TopicName = topicName;
        }
    }
}