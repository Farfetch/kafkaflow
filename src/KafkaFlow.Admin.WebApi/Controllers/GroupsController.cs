namespace KafkaFlow.Admin.WebApi.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using KafkaFlow.Admin.Messages;
    using KafkaFlow.Admin.WebApi.Contracts;
    using KafkaFlow.Consumers;
    using Microsoft.AspNetCore.Mvc;

    [Route("kafka-flow/groups")]
    [ApiController]
    public class GroupsController : ControllerBase
    {
        private readonly IConsumerAccessor consumers;
        private readonly IAdminProducer adminProducer;

        public GroupsController(
            IConsumerAccessor consumers,
            IAdminProducer adminProducer)
        {
            this.consumers = consumers;
            this.adminProducer = adminProducer;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<GroupResponse>), 200)]
        public IActionResult Get()
        {
            return this.Ok(
                this.consumers.All
                    .GroupBy(x => x.GroupId)
                    .Select(
                        x => new GroupResponse
                        {
                            GroupId = x.First().GroupId,
                            Consumers = x.Select(y => y),
                        }));
        }

        [HttpPost]
        [Route("{groupId}/pause")]
        [ProducesResponseType(202)]
        public async Task<IActionResult> Pause([FromRoute] string groupId)
        {
            await this.adminProducer.ProduceAsync(
                new PauseConsumersByGroup
                {
                    GroupId = groupId,
                });

            return this.Accepted();
        }

        [HttpPost]
        [Route("{groupId}/resume")]
        [ProducesResponseType(202)]
        public async Task<IActionResult> Resume([FromRoute] string groupId)
        {
            await this.adminProducer.ProduceAsync(
                new ResumeConsumersByGroup
                {
                    GroupId = groupId,
                });

            return this.Accepted();
        }
    }
}
