namespace KafkaFlow.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Used to build the consumer configuration
    /// </summary>
    public interface IConsumerConfigurationBuilder
    {
        /// <summary>
        /// Gets the dependency injection configurator
        /// </summary>
        IDependencyConfigurator DependencyConfigurator { get; }

        /// <summary>
        /// Sets the topic that will be used to read the messages, the partitions will be automatically assigned
        /// </summary>
        /// <param name="topicName">Topic name</param>
        /// <returns></returns>
        IConsumerConfigurationBuilder Topic(string topicName);

        /// <summary>
        /// Explicitly defines the topic and partitions that will be used to read the messages
        /// </summary>
        /// <param name="topicName">Topic name</param>
        /// <param name="partitions">The partitions IDs</param>
        /// <returns></returns>
        IConsumerConfigurationBuilder ManualAssignPartitions(string topicName, IEnumerable<int> partitions);

        /// <summary>
        /// Sets the topics that will be used to read the messages, the partitions will be automatically assigned
        /// </summary>
        /// <param name="topicNames">Topic names</param>
        /// <returns></returns>
        IConsumerConfigurationBuilder Topics(IEnumerable<string> topicNames);

        /// <summary>
        /// Sets the topics that will be used to read the messages, the partitions will be automatically assigned
        /// </summary>
        /// <param name="topicNames">Topic names</param>
        /// <returns></returns>
        IConsumerConfigurationBuilder Topics(params string[] topicNames);

        /// <summary>
        /// Sets a unique name for the consumer
        /// </summary>
        /// <param name="name">A unique name</param>
        /// <returns></returns>
        IConsumerConfigurationBuilder WithName(string name);

        /// <summary>
        /// Sets the consumer as readonly, that means this consumer can not be managed and it will not send telemetry data
        /// </summary>
        /// <returns></returns>
        IConsumerConfigurationBuilder DisableManagement();

        /// <summary>
        /// Sets the group id used by the consumer
        /// </summary>
        /// <param name="groupId">The consumer id value</param>
        /// <returns></returns>
        IConsumerConfigurationBuilder WithGroupId(string groupId);

        /// <summary>
        /// Sets the initial offset strategy used by new consumer groups.
        /// If your consumer group (set by method <see cref="WithGroupId(string)"/>) has no offset stored in Kafka, this configuration will be used
        /// </summary>
        /// <param name="autoOffsetReset">The <see cref="AutoOffsetReset"/> enum value</param>
        /// <returns></returns>
        IConsumerConfigurationBuilder WithAutoOffsetReset(AutoOffsetReset autoOffsetReset);

        /// <summary>
        /// Sets the interval used by the framework to commit the stored offsets in Kafka
        /// </summary>
        /// <param name="autoCommitIntervalMs">The interval in milliseconds</param>
        /// <returns></returns>
        IConsumerConfigurationBuilder WithAutoCommitIntervalMs(int autoCommitIntervalMs);

        /// <summary>
        /// Sets the max interval between message consumption, if this time exceeds the consumer is considered failed and Kafka will revoke the assigned partitions
        /// </summary>
        /// <param name="maxPollIntervalMs">The interval in milliseconds</param>
        /// <returns></returns>
        IConsumerConfigurationBuilder WithMaxPollIntervalMs(int maxPollIntervalMs);

        /// <summary>
        /// Sets the number of threads that will be used to consume the messages
        /// </summary>
        /// <param name="workersCount">The number of workers</param>
        /// <returns></returns>
        IConsumerConfigurationBuilder WithWorkersCount(int workersCount);

        /// <summary>
        /// Configures a custom function to dynamically calculate the number of workers.
        /// This function is called at intervals specified by the WithWorkersCountEvaluationInterval method.
        /// </summary>
        /// <param name="calculator">A function that takes a WorkersCountContext object and returns a Task yielding the new workers count</param>
        /// <returns>The IConsumerConfigurationBuilder instance for method chaining</returns>
        IConsumerConfigurationBuilder WithWorkersCount(Func<WorkersCountContext, IDependencyResolver, Task<int>> calculator);

        /// <summary>
        /// Configures the interval at which the workers count should be evaluated using a TimeSpan.
        /// This enables dynamic adjustment of workers count based on changing conditions.
        /// </summary>
        /// <param name="interval">The time interval for re-evaluating the workers count</param>
        /// <returns>The IConsumerConfigurationBuilder instance for method chaining</returns>
        IConsumerConfigurationBuilder WithWorkersCountEvaluationInterval(TimeSpan interval);

        /// <summary>
        /// Configures the interval at which the workers count should be evaluated using minutes.
        /// This enables dynamic adjustment of workers count based on changing conditions.
        /// </summary>
        /// <param name="minutes">The time interval in minutes for re-evaluating the workers count</param>
        /// <returns>The IConsumerConfigurationBuilder instance for method chaining</returns>
        IConsumerConfigurationBuilder WithWorkersCountEvaluationInterval(int minutes);

        /// <summary>
        /// Sets how many messages will be buffered for each worker
        /// </summary>
        /// <param name="size">The buffer size</param>
        /// <returns></returns>
        IConsumerConfigurationBuilder WithBufferSize(int size);

        /// <summary>
        /// Sets the time that the worker will wait to process the buffered messages
        /// before canceling the <see cref="IConsumerContext.WorkerStopped"/>
        /// </summary>
        /// <param name="seconds">The seconds to wait</param>
        /// <returns></returns>
        IConsumerConfigurationBuilder WithWorkerStopTimeout(int seconds);

        /// <summary>
        /// Sets the time that the worker will wait to process the buffered messages
        /// before canceling the <see cref="IConsumerContext.WorkerStopped"/>
        /// </summary>
        /// <param name="timeout">The time to wait</param>
        /// <returns></returns>
        IConsumerConfigurationBuilder WithWorkerStopTimeout(TimeSpan timeout);

        /// <summary>
        /// Sets the strategy to choose a worker when a message arrives
        /// </summary>
        /// <typeparam name="T">A class that implements the <see cref="IDistributionStrategy"/> interface</typeparam>
        /// <param name="factory">A factory to create the instance</param>
        /// <returns></returns>
        IConsumerConfigurationBuilder WithWorkDistributionStrategy<T>(Factory<T> factory)
            where T : class, IDistributionStrategy;

        /// <summary>
        /// Sets the strategy to choose a worker when a message arrives
        /// </summary>
        /// <typeparam name="T">A class that implements the <see cref="IDistributionStrategy"/> interface</typeparam>
        /// <returns></returns>
        IConsumerConfigurationBuilder WithWorkDistributionStrategy<T>()
            where T : class, IDistributionStrategy;

        /// <summary>
        /// Offsets will be stored after the execution of the handler and middlewares automatically, this is the default behaviour
        /// </summary>
        /// <returns></returns>
        IConsumerConfigurationBuilder WithAutoStoreOffsets();

        /// <summary>
        /// The client should call the <see cref="IConsumerContext.StoreOffset()"/>
        /// </summary>
        /// <returns></returns>
        IConsumerConfigurationBuilder WithManualStoreOffsets();

        /// <summary>
        /// No offsets will be stored on Kafka
        /// </summary>
        /// <returns></returns>
        IConsumerConfigurationBuilder WithoutStoringOffsets();

        /// <summary>
        /// Configures the consumer initial state.The default is <see cref="ConsumerInitialState.Running"/>
        /// </summary>
        /// <param name="state">The consumer state</param>
        /// <returns></returns>
        IConsumerConfigurationBuilder WithInitialState(ConsumerInitialState state);

        /// <summary>
        /// Adds middlewares to the consumer. The middlewares will be executed in the registration order
        /// </summary>
        /// <param name="middlewares">A handler to register middlewares</param>
        /// <returns></returns>
        IConsumerConfigurationBuilder AddMiddlewares(Action<IConsumerMiddlewareConfigurationBuilder> middlewares);

        /// <summary>
        /// Adds a handler for the Kafka consumer statistics
        /// </summary>
        /// <param name="statisticsHandler">A handler for the statistics</param>
        /// <returns></returns>
        IConsumerConfigurationBuilder WithStatisticsHandler(Action<string> statisticsHandler);

        /// <summary>
        /// Sets the interval the statistics are emitted
        /// </summary>
        /// <param name="statisticsIntervalMs">The interval in milliseconds</param>
        /// <returns></returns>
        IConsumerConfigurationBuilder WithStatisticsIntervalMs(int statisticsIntervalMs);
    }
}
