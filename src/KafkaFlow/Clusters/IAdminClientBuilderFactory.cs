using Confluent.Kafka;

namespace KafkaFlow.Clusters;

/// <summary>
/// Factory for creating an <see cref="AdminClientBuilder"/>.
/// </summary>
public interface IAdminClientBuilderFactory
{
    /// <summary>
    /// Creates an instance of <see cref="AdminClientBuilder"/>.
    /// </summary>
    /// <param name="config">The admin client config</param>
    /// <returns>An <see cref="AdminClientBuilder"/>.</returns>
    IAdminClientBuilder CreateAdminClientBuilder(AdminClientConfig config);
}