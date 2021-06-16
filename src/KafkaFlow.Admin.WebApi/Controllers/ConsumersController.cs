namespace KafkaFlow.Admin.WebApi.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using KafkaFlow.Admin.Messages;
    using KafkaFlow.Admin.WebApi.Adapters;
    using KafkaFlow.Admin.WebApi.Contracts;
    using KafkaFlow.Consumers;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Consumers controller
    /// </summary>
    [Route("kafka-flow/groups/{groupId}/consumers")]
    [ApiController]
    public class ConsumersController : ControllerBase
    {
        private readonly IConsumerAccessor consumers;
        private readonly IAdminProducer adminProducer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsumersController"/> class.
        /// </summary>
        /// <param name="consumers">The accessor class that provides access to the consumers</param>
        /// <param name="adminProducer">The producer to publish admin messages</param>
        public ConsumersController(IConsumerAccessor consumers, IAdminProducer adminProducer)
        {
            this.consumers = consumers;
            this.adminProducer = adminProducer;
        }

        /// <summary>
        /// Get the consumers with the group id provided
        /// </summary>
        /// <param name="groupId">Identifier of the group</param>
        /// <returns>A list of consumers</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ConsumersResponse), 200)]
        public IActionResult Get([FromRoute] string groupId)
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
        [Route("{consumerName}")]
        [ProducesResponseType(typeof(ConsumerResponse), 200)]
        [ProducesResponseType(404)]
        public IActionResult Get(
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
        /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
        [HttpPost]
        [Route("{consumerName}/pause")]
        [ProducesResponseType(202)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Pause(
            [FromRoute] string groupId,
            [FromRoute] string consumerName)
        {
            var consumer = this.consumers.All
                .FirstOrDefault(x => x.GroupId == groupId && x.ConsumerName == consumerName);

            if (consumer is null)
            {
                return this.NotFound();
            }

            await this.adminProducer.ProduceAsync(
                new PauseConsumerByName
                {
                    ConsumerName = consumerName,
                });

            return this.Accepted();
        }

        /// <summary>
        /// Resume the consumer based on the provided filters
        /// </summary>
        /// <param name="groupId">Identifier of the group</param>
        /// <param name="consumerName">Name of consumer</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
        [HttpPost]
        [Route("{consumerName}/resume")]
        [ProducesResponseType(202)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Resume(
            [FromRoute] string groupId,
            [FromRoute] string consumerName)
        {
            var consumer = this.consumers.All
                .FirstOrDefault(x => x.GroupId == groupId && x.ConsumerName == consumerName);

            if (consumer is null)
            {
                return this.NotFound();
            }

            await this.adminProducer.ProduceAsync(
                new ResumeConsumerByName
                {
                    ConsumerName = consumerName,
                });

            return this.Accepted();
        }

        /// <summary>
        /// Restart the consumer based on the provided filters
        /// </summary>
        /// <param name="groupId">Identifier of the group</param>
        /// <param name="consumerName">Name of consumer</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
        [HttpPost]
        [Route("{consumerName}/restart")]
        [ProducesResponseType(202)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Restart(
            [FromRoute] string groupId,
            [FromRoute] string consumerName)
        {
            var consumer = this.consumers.All
                .FirstOrDefault(x => x.GroupId == groupId && x.ConsumerName == consumerName);

            if (consumer is null)
            {
                return this.NotFound();
            }

            await this.adminProducer.ProduceAsync(
                new RestartConsumerByName
                {
                    ConsumerName = consumerName,
                });

            return this.Accepted();
        }

        /// <summary>
        /// Reset the consumer partitions offset
        /// </summary>
        /// <param name="groupId">Identifier of the group</param>
        /// <param name="consumerName">Name of consumer</param>
        /// <param name="request">The request to confirm the operation</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
        [HttpPost]
        [Route("{consumerName}/reset-offsets")]
        [ProducesResponseType(202)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> ResetOffsets(
            [FromRoute] string groupId,
            [FromRoute] string consumerName,
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

            await this.adminProducer.ProduceAsync(
                new ResetConsumerOffset
                {
                    ConsumerName = consumerName,
                });

            return this.Accepted();
        }

        /// <summary>
        /// Rewind the consumer partitions offset to a point in time
        /// </summary>
        /// <param name="groupId">Identifier of the group</param>
        /// <param name="consumerName">Name of consumer</param>
        /// <param name="request">The request to confirm the operation</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
        [HttpPost]
        [Route("{consumerName}/rewind-offsets-to-date")]
        [ProducesResponseType(202)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> RewindOffsetsToDate(
            [FromRoute] string groupId,
            [FromRoute] string consumerName,
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

            await this.adminProducer.ProduceAsync(
                new RewindConsumerOffsetToDateTime
                {
                    ConsumerName = consumerName,
                    DateTime = request.Date,
                });

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
        [Route("{consumerName}/change-worker-count")]
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

            await this.adminProducer.ProduceAsync(
                new ChangeConsumerWorkersCount
                {
                    ConsumerName = consumerName,
                    WorkersCount = request.WorkersCount,
                });

            return this.Accepted();
        }
    }
}