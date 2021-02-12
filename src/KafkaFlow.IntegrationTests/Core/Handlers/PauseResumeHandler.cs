namespace KafkaFlow.IntegrationTests.Core.Handlers
{
    using System.Threading.Tasks;
    using KafkaFlow.TypedHandler;
    using Messages;

    public class PauseResumeHandler : IMessageHandler<TestMessage1>
    {
        public async Task Handle(IMessageContext context, TestMessage1 message)
        {
            context.Consumer.Pause();
            
            await Task.Delay(Bootstrapper.MaxPollIntervalMs + 1000);
            
            MessageStorage.Add(message);
            
            context.Consumer.Resume();
        }
    }
}
