using System;

namespace KafkaFlow.Events
{
    // Isto devia ser internal visto que a notificação de events não deveria ser publica, apenas a subscrição desses eventos

    // Falta o OnConsumeEnd e OnProduceEnd
    public interface IEventsNotifier
    {
        void NotifyOnConsumeError(Exception exception);

        void NotifyOnConsumeStart(IMessageContext context);

        void NotifyOnProduceError(Exception exception);

        void NotifyOnProduceStart(IMessageContext context);
    }
}
