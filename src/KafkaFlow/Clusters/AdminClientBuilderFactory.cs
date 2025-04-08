using Confluent.Kafka;

namespace KafkaFlow.Clusters;

internal class AdminClientBuilderFactory : IAdminClientBuilderFactory
{
    public IAdminClientBuilder CreateAdminClientBuilder(AdminClientConfig config)
    {
        return new AdminClientBuilderWrapper(config);
    }
}