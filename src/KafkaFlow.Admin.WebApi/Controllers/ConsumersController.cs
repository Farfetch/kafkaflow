namespace KafkaFlow.Admin.WebApi.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using KafkaFlow.Admin.WebApi.Adapters;
    using KafkaFlow.Admin.WebApi.Contracts;
    using KafkaFlow.Consumers;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Consumers controller
    /// </summary>
    [Route("kafkaflow/groups/{groupId}/consumers")]
    [ApiController]
    public class ConsumersController : ControllerBase
    {
        private readonly IConsumerAccessor consumers;
        private readonly IConsumerAdmin consumerAdmin;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsumersController"/> class.
        /// </summary>
        /// <param name="consumers">The accessor class that provides access to the consumers</param>
        /// <param name="consumerAdmin">The admin messages consumer</param>
        public ConsumersController(IConsumerAccessor consumers, IConsumerAdmin consumerAdmin)
        {
            this.consumers = consumers;
            this.consumerAdmin = consumerAdmin;
        }

        /// <summary>
        /// Get the consumers with the group id provided
        /// </summary>
        /// <param name="groupId">Identifier of the group</param>
        /// <returns>A list of consumers</returns>
        [HttpGet(Name = nameof(GetConsumersByGroupId))]
        [ProducesResponseType(typeof(ConsumersResponse), 200)]
        public IActionResult GetConsumersByGroupId([FromRoute] string groupId)
        {
            return this.Ok(
                new ConsumersResponse
                {
                    Consumers = this.consumers
                        .All
                        .Where(x => x.GroupId == groupId)
                        .Select(x => x.Adapt()),
                });
        }

        /// <summary>
        /// Get the consumer based on the provided filters
        /// </summary>
        /// <param name="groupId">Identifier of the group</param>
        /// <param name="consumerName">Name of consumer</param>
        /// <returns>A list of consumers</returns>
        [HttpGet]
        [Route("{consumerName}", Name = nameof(GetConsumerByGroupIdName))]
        [ProducesResponseType(typeof(ConsumerResponse), 200)]
        [ProducesResponseType(404)]
        public IActionResult GetConsumerByGroupIdName(
            [FromRoute] string groupId,
            [FromRoute] string consumerName)
        {
            var consumer = this.consumers.All
                .FirstOrDefault(x => x.GroupId == groupId && x.ConsumerName == consumerName);

            if (consumer is null)
            {
                return this.NotFound();
            }

            return this.Ok(consumer.Adapt());
        }

        /// <summary>
        /// Pause the consumer based on the provided filters
        /// </summary>
        /// <param name="groupId">Identifier of the group</param>
        /// <param name="consumerName">Name of consumer</param>
        /// <param name="topics">List of topics</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
        [HttpPost]
        [Route("{consumerName}/pause", Name = nameof(PauseConsumer))]
        [ProducesResponseType(202)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> PauseConsumer(
            [FromRoute] string groupId,
            [FromRoute] string consumerName,
            [FromQuery] IList<string> topics)
        {
            var consumer = this.consumers.All
                .FirstOrDefault(x => x.GroupId == groupId && x.ConsumerName == consumerName);

            if (consumer is null)
            {
                return this.NotFound();
            }

            await this.consumerAdmin.PauseConsumerAsync(consumerName, topics);

            return this.Accepted();
        }

        /// <summary>
        /// Resume the consumer based on the provided filters
        /// </summary>
        /// <param name="groupId">Identifier of the group</param>
        /// <param name="consumerName">Name of consumer</param>
        /// <param name="topics">List of topics</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
        [HttpPost]
        [Route("{consumerName}/resume", Name = nameof(ResumeConsumer))]
        [ProducesResponseType(202)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ResumeConsumer(
            [FromRoute] string groupId,
            [FromRoute] string consumerName,
            [FromQuery] IList<string> topics)
        {
            var consumer = this.consumers.All
                .FirstOrDefault(x => x.GroupId == groupId && x.ConsumerName == consumerName);

            if (consumer is null)
            {
                return this.NotFound();
            }

            await this.consumerAdmin.ResumeConsumerAsync(consumerName, topics);

            return this.Accepted();
        }

        /// <summary>
        /// Starts a consumer
        /// </summary>
        /// <param name="groupId">Identifier of the group</param>
        /// <param name="consumerName">Name of consumer</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
        [HttpPost]
        [Route("{consumerName}/start", Name = nameof(StartConsumer))]
        [ProducesResponseType(202)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> StartConsumer(
            [FromRoute] string groupId,
            [FromRoute] string consumerName)
        {
            var consumer = this.consumers.All.FirstOrDefault(x => x.GroupId == groupId && x.ConsumerName == consumerName);

            if (consumer is null)
            {
                return this.NotFound();
            }

            await this.consumerAdmin.StartConsumerAsync(consumerName);

            return this.Accepted();
        }

        /// <summary>
        /// Stops a consumer
        /// </summary>
        /// <param name="groupId">Identifier of the group</param>
        /// <param name="consumerName">Name of consumer</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
        [HttpPost]
        [Route("{consumerName}/stop", Name = nameof(StopConsumer))]
        [ProducesResponseType(202)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> StopConsumer(
            [FromRoute] string groupId,
            [FromRoute] string consumerName)
        {
            var consumer = this.consumers.All.FirstOrDefault(x => x.GroupId == groupId && x.ConsumerName == consumerName);

            if (consumer is null)
            {
                return this.NotFound();
            }

            await this.consumerAdmin.StopConsumerAsync(consumerName);

            return this.Accepted();
        }

        /// <summary>
        /// Restart the consumer based on the provided filters
        /// </summary>
        /// <param name="groupId">Identifier of the group</param>
        /// <param name="consumerName">Name of consumer</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
        [HttpPost]
        [Route("{consumerName}/restart", Name = nameof(RestartConsumer))]
        [ProducesResponseType(202)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> RestartConsumer(
            [FromRoute] string groupId,
            [FromRoute] string consumerName)
        {
            var consumer = this.consumers.All
                .FirstOrDefault(x => x.GroupId == groupId && x.ConsumerName == consumerName);

            if (consumer is null)
            {
                return this.NotFound();
            }

            await this.consumerAdmin.RestartConsumerAsync(consumerName);

            return this.Accepted();
        }

        /// <summary>
        /// Reset the consumer partitions offset
        /// </summary>
        /// <param name="groupId">Identifier of the group</param>
        /// <param name="consumerName">Name of consumer</param>
        /// <param name="topics">List of topics</param>
        /// <param name="request">The request to confirm the operation</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
        [HttpPost]
        [Route("{consumerName}/reset-offsets", Name = nameof(ResetOffsets))]
        [ProducesResponseType(202)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> ResetOffsets(
            [FromRoute] string groupId,
            [FromRoute] string consumerName,
            [FromQuery] IList<string> topics,
            [FromBody] ResetOffsetsRequest request)
        {
            if (request?.Confirm == false)
            {
                return this.BadRequest();
            }

            var consumer = this.consumers.All
                .FirstOrDefault(x => x.GroupId == groupId && x.ConsumerName == consumerName);

            if (consumer is null)
            {
                return this.NotFound();
            }

            await this.consumerAdmin.ResetOffsetsAsync(consumerName, topics);

            return this.Accepted();
        }

        /// <summary>
        /// Rewind the consumer partitions offset to a point in time
        /// </summary>
        /// <param name="groupId">Identifier of the group</param>
        /// <param name="consumerName">Name of consumer</param>
        /// <param name="topics">List of topics</param>
        /// <param name="request">The request to confirm the operation</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
        [HttpPost]
        [Route("{consumerName}/rewind-offsets-to-date", Name = nameof(RewindOffsets))]
        [ProducesResponseType(202)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> RewindOffsets(
            [FromRoute] string groupId,
            [FromRoute] string consumerName,
            [FromQuery] IList<string> topics,
            [FromBody] RewindOffsetsToDateRequest request)
        {
            if (request is null)
            {
                return this.BadRequest();
            }

            var consumer = this.consumers.All
                .FirstOrDefault(x => x.GroupId == groupId && x.ConsumerName == consumerName);

            if (consumer is null)
            {
                return this.NotFound();
            }

            await this.consumerAdmin.RewindOffsetsAsync(consumerName, request.Date, topics);

            return this.Accepted();
        }

        /// <summary>
        /// Change the number of workers running in the consumer
        /// </summary>
        /// <param name="groupId">Identifier of the group</param>
        /// <param name="consumerName">Name of consumer</param>
        /// <param name="request">The request to confirm the operation</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
        [HttpPost]
        [Route("{consumerName}/change-worker-count", Name = nameof(ChangeWorkersCount))]
        [ProducesResponseType(202)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> ChangeWorkersCount(
            [FromRoute] string groupId,
            [FromRoute] string consumerName,
            [FromBody] ChangeWorkersCountRequest request)
        {
            if (request is null || request.WorkersCount <= 0)
            {
                return this.BadRequest();
            }

            var consumer = this.consumers.All
                .FirstOrDefault(x => x.GroupId == groupId && x.ConsumerName == consumerName);

            if (consumer is null)
            {
                return this.NotFound();
            }

            await this.consumerAdmin.ChangeWorkersCountAsync(consumerName, request.WorkersCount);

            return this.Accepted();
        }
    }
}
