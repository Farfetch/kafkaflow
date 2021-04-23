namespace KafkaFlow.Admin.WebApi.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using KafkaFlow.Admin.Messages;
    using KafkaFlow.Admin.WebApi.Contracts;
    using KafkaFlow.Consumers;
    using Microsoft.AspNetCore.Mvc;

    [Route("kafka-flow/groups/{groupId}/consumers")]
    [ApiController]
    internal class ConsumersController : ControllerBase
    {
        private readonly IConsumerAccessor consumers;
        private readonly IAdminProducer adminProducer;

        public ConsumersController(IConsumerAccessor consumers, IAdminProducer adminProducer)
        {
            this.consumers = consumers;
            this.adminProducer = adminProducer;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<IMessageConsumer>), 200)]
        public IActionResult Get([FromRoute] string groupId)
        {
            return this.Ok(this.consumers.All.Where(x => x.GroupId == groupId));
        }

        [HttpGet]
        [Route("{consumerName}")]
        [ProducesResponseType(typeof(IMessageConsumer), 200)]
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

            return this.Ok(consumer);
        }

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

        [HttpPost]
        [Route("{consumerName}/change-worker-count")]
        [ProducesResponseType(202)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> ChangeWorkerCount(
            [FromRoute] string groupId,
            [FromRoute] string consumerName,
            [FromBody] ChangeWorkerCountRequest request)
        {
            if (request is null || request.WorkerCount <= 0)
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
                new ChangeConsumerWorkerCount
                {
                    ConsumerName = consumerName,
                    WorkerCount = request.WorkerCount,
                });

            return this.Accepted();
        }
    }
}
