namespace KafkaFlow;

/// <summary>
/// Consumer initial state options
/// </summary>
public enum ConsumerInitialState
{
    /// <summary>
    /// Automatically start the consumer when the bus is started
    /// </summary>
    Running,

    /// <summary>
    /// Do not start the consumer when the bus is started
    /// </summary>
    Stopped,
}
