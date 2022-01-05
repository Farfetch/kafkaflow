namespace KafkaFlow.Client.Protocol.Messages.Implementations
{
    using System.Collections.Generic;

    public interface IOffsetCommitRequest : IRequestMessage<IOffsetCommitResponse>
    {
        string GroupId { get; set; }

        int GroupGenerationId { get; set; }

        string MemberId { get; set; }

        long RetentionTimeMs { get; set; }

        List<ITopic> Topics { get; }

        ITopic AddTopic();

        public interface ITopic : IRequest
        {
            string Name { get; set; }

            List<IPartition> Partitions { get; }

            IPartition AddPartition();
        }

        public interface IPartition : IRequest
        {
            int Id { get; set; }

            long Offset { get; set; }

            string Metadata { get; set; }
        }
    }
}
