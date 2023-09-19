namespace KafkaFlow.IntegrationTests.Core.Handlers
{
    using System.Threading.Tasks;
    using KafkaFlow.IntegrationTests.Core.Messages;
    using KafkaFlow.Middlewares.TypedHandler;

    internal class PauseResumeHandler : IMessageHandler<PauseResumeMessage>
    {
        public async Task Handle(IMessageContext context, PauseResumeMessage message)
        {
            context.ConsumerContext.Pause();

            await Task.Delay(Bootstrapper.MaxPollIntervalMs + 1000);

            MessageStorage.Add(message);

            context.ConsumerContext.Resume();
        }
    }
}
