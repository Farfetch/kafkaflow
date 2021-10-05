namespace KafkaFlow.Extensions.ProducerThrottling.Evaluations
{
    using System.Threading.Tasks;

    /// <summary>
    /// An interface used to create threshold evaluations
    /// </summary>
    public interface IEvaluation
    {
        /// <summary>
        /// The evaluation function
        /// </summary>
        /// <returns></returns>
        Task<long> Eval();
    }
}
