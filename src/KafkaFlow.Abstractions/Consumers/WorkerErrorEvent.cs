namespace KafkaFlow.Consumers
{
    using KafkaFlow;
    using System;
    using System.Threading.Tasks;

    public class WorkerErrorEvent : Event<Exception>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkerErrorEvent"/> class.
        /// </summary>
        /// <param name="logHandler">The log handler object to be used</param>
       

        public void Subscribe(Func<Exception, Task> handler)
        {
            throw new NotImplementedException();
        }
    }
}
