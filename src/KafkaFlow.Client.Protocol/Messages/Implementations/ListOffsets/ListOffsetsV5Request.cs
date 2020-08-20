namespace KafkaFlow.Client.Protocol.Messages.Implementations.ListOffsets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using KafkaFlow.Client.Protocol.Streams;

    /// <summary>
    /// Request message to achieve the list of offsets in a topic
    /// </summary>
    public class ListOffsetsV5Request : IListOffsetsRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListOffsetsV5Request"/> class.
        /// </summary>
        /// <param name="replicaId">The broker ID of the requestor, or -1 if this request is being made by a normal consumer.</param>
        /// <param name="isolationLevel">This setting controls the visibility of transactional records. Using READ_UNCOMMITTED (isolation_level = 0) makes all records visible. With READ_COMMITTED (isolation_level = 1), non-transactional and COMMITTED transactional records are visible. To be more concrete, READ_COMMITTED returns all data from offsets smaller than the current LSO (last stable offset), and enables the inclusion of the list of aborted transactions in the result, which allows consumers to discard ABORTED transactional records</param>
        /// <param name="topicName">The topic name.</param>
        /// <param name="partitions">Each partition in the request.</param>
        public ListOffsetsV5Request(int replicaId, byte isolationLevel, string topicName, int[] partitions)
        {
            this.ReplicaId = replicaId;
            this.IsolationLevel = isolationLevel;
            this.Topics = new[] { this.CreateTopic(topicName, partitions) };
        }

        /// <inheritdoc/>
        public ApiKey ApiKey => ApiKey.ListOffsets;

        /// <inheritdoc/>
        public short ApiVersion => 5;

        /// <inheritdoc/>
        public int ReplicaId { get; }

        /// <inheritdoc/>
        public byte IsolationLevel { get; }

        /// <inheritdoc/>
        public IListOffsetsRequest.ITopic[] Topics { get; }

        /// <inheritdoc/>
        public Type ResponseType => typeof(ListOffsetsV5Response);

        /// <inheritdoc/>
        void IRequest.Write(MemoryWriter destination)
        {
            destination.WriteInt32(this.ReplicaId);
            destination.WriteByte(this.IsolationLevel);
            destination.WriteArray(this.Topics);
        }

        /// <summary>
        /// Create a new <see cref="IListOffsetsRequest.ITopic"/> instance
        /// </summary>
        /// <param name="name">Name of the topic</param>
        /// <param name="partitions">List of partitions</param>
        /// <returns>A new instance of <see cref="IListOffsetsRequest.ITopic"/></returns>
        private IListOffsetsRequest.ITopic CreateTopic(string name, int[] partitions) => new Topic(name, partitions);

        /// <summary>
        /// Represents a topic
        /// </summary>
        private class Topic : IListOffsetsRequest.ITopic
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Topic"/> class.
            /// </summary>
            /// <param name="name">Name of the topic</param>
            /// <param name="partitions">List of partitions</param>
            public Topic(string name, IEnumerable<int> partitions)
            {
                this.Name = name;
                this.Partitions = partitions.Select(this.CreatePartition).ToArray();
            }

            /// <inheritdoc/>
            public string Name { get; }

            /// <inheritdoc/>
            public IListOffsetsRequest.IPartition[] Partitions { get; }

            /// <inheritdoc/>
            void IRequest.Write(MemoryWriter destination)
            {
                destination.WriteString(this.Name);
                destination.WriteArray(this.Partitions);
            }

            /// <summary>
            /// Create a new partition instance
            /// </summary>
            /// <param name="partition">The partition number</param>
            /// <returns></returns>
            private IListOffsetsRequest.IPartition CreatePartition(int partition) => new Partition(partition);
        }

        /// <summary>
        /// Represents a partition
        /// </summary>
        private class Partition : IListOffsetsRequest.IPartition
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Partition"/> class
            /// </summary>
            /// <param name="partitionIndex">Partition index</param>
            public Partition(int partitionIndex)
            {
                this.PartitionIndex = partitionIndex;
                this.CurrentLeaderEpoch = 0;
                this.Timestamp = -1;
            }

            /// <inheritdoc/>
            public int PartitionIndex { get; }

            /// <inheritdoc/>
            public int CurrentLeaderEpoch { get; }

            /// <inheritdoc/>
            public long Timestamp { get; }

            /// <inheritdoc/>
            void IRequest.Write(MemoryWriter destination)
            {
                destination.WriteInt32(this.PartitionIndex);
                destination.WriteInt32(this.CurrentLeaderEpoch);
                destination.WriteInt64(this.Timestamp);
            }
        }
    }
}
