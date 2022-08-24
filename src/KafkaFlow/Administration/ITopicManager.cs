namespace KafkaFlow.Administration
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using KafkaFlow.Configuration;

    internal interface ITopicManager
    {
        public Task CreateIfNotExistsAsync(string servers, IEnumerable<TopicConfiguration> configuration);
    }
}
