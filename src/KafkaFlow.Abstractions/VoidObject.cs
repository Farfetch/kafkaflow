namespace KafkaFlow;

/// <summary>
/// A type that represents an empty object that should be ignored
/// </summary>
public class VoidObject
{
    /// <summary>
    /// Gets the unique instance value
    /// </summary>
    public static readonly VoidObject Value = new();

    private VoidObject()
    {
        // Empty
    }
}
