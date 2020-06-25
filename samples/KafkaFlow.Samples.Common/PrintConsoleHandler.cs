namespace KafkaFlow.Samples.Common
{
    using System;
    using System.Threading.Tasks;
    using KafkaFlow.TypedHandler;

    public class PrintConsoleHandler
        : IMessageHandler<TestMessage>,
            IMessageHandler<TestMessage2>
    {
        public Task Handle(IMessageContext context, TestMessage message)
        {
            Console.WriteLine(message.Text);
            return Task.CompletedTask;
        }

        public Task Handle(IMessageContext context, TestMessage2 message)
        {
            var watermark = context.Consumer.GetOffsetsWatermark();

            Console.WriteLine(
                "Watermark: {0} | Offset: {1} | Lag: {2} | Message: {3}",
                watermark.High,
                context.Offset,
                watermark.High - context.Offset.Value - 1,
                message.Value);

            return Task.CompletedTask;
        }
    }
}
