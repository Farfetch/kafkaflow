namespace KafkaFlow
{
    /// <summary>
    /// Specifies the lifetime of a class
    /// </summary>
    public enum InstanceLifetime
    {
        /// <summary>
        /// Specifies that a single instance of the class will be created
        /// </summary>
        Singleton,

        /// <summary>
        /// Specifies that a new instance of the class will be created for each scope
        /// </summary>
        Scoped,

        /// <summary>
        /// Specifies that a new instance of the class will be created every time it is requested
        /// </summary>
        Transient,
    }
}
