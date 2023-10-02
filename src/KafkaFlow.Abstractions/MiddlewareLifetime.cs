namespace KafkaFlow
{
    /// <summary>
    /// Specifies the lifetime of a middleware
    /// </summary>
    public enum MiddlewareLifetime
    {
        /// <summary>
        /// Denotes a single instance of the middleware to be maintained throughout the application's lifecycle.
        /// This instance will not be disposed until the application is shut down.
        /// </summary>
        Singleton,

        /// <summary>
        /// Indicates a new middleware instance is instantiated for each individual message scope, ensuring isolated processing.
        /// This instance will be disposed when the message scope ends.
        /// </summary>
        Message,

        /// <summary>
        /// Signifies a new middleware instance is created every time a request is made, providing the highest level of isolation.
        /// This instance will be disposed as soon as it goes out of scope.
        /// </summary>
        Transient,

        /// <summary>
        /// Specifies a separate middleware instance for each Consumer/Producer, useful for maintaining context within the Consumer/Producer scope.
        /// This instance will be disposed when the Consumer/Producer it belongs to is disposed.
        /// </summary>
        ConsumerOrProducer,

        /// <summary>
        /// Specifies a unique middleware instance for each Consumer Worker. For Producers, this behaves as ConsumerOrProducer. This allows context preservation at a worker level.
        /// This instance will be disposed when the Worker it belongs to is disposed.
        /// </summary>
        Worker,
    }
}
