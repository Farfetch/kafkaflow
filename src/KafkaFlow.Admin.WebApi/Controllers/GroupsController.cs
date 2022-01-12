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
    /// Groups controller
    /// </summary>
    [Route("kafka-flow/groups")]
    [ApiController]
    public class GroupsController : ControllerBase
    {
        private readonly IConsumerAccessor consumers;
        private readonly IAdminProducer adminProducer;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupsController"/> class.
        /// </summary>
        /// <param name="consumers">The accessor class that provides access to the consumers</param>
        /// <param name="adminProducer">The producer to publish admin messages</param>
        public GroupsController(IConsumerAccessor consumers, IAdminProducer adminProducer)
        {
            this.consumers = consumers;
            this.adminProducer = adminProducer;
        }

        /// <summary>
        /// Get all the consumer groups
        /// </summary>
        /// <returns>A list of consumer groups</returns>
        [HttpGet(Name=nameof(GetAllGroups))]
        [ProducesResponseType(typeof(GroupsResponse), 200)]
        public IActionResult GetAllGroups()
        {
            return this.Ok(
                new GroupsResponse
                {
                    Groups = this.consumers.All
                        .GroupBy(x => x.GroupId)
                        .Select(
                            x => new GroupResponse
                            {
                                GroupId = x.First().GroupId,
                                Consumers = x.Select(y => y.Adapt()),
                            }),
                });
        }

        /// <summary>
        /// Pause all consumers from a specific group
        /// </summary>
        /// <param name="groupId">Identifier of the group</param>
        /// <param name="topics">List of topics</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
        [HttpPost]
        [Route("{groupId}/pause", Name=nameof(PauseGroup))]
        [ProducesResponseType(202)]
        public async Task<IActionResult> PauseGroup(
            [FromRoute] string groupId,
            [FromQuery] IList<string> topics)
        {
            await this.adminProducer.ProduceAsync(
                new PauseConsumersByGroup
                {
                    GroupId = groupId,
                    Topics = topics,
                });

            return this.Accepted();
        }

        /// <summary>
        /// Resume all consumers from a specific group
        /// </summary>
        /// <param name="groupId">Identifier of the group</param>
        /// <param name="topics">List of topics</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
        [HttpPost]
        [Route("{groupId}/resume", Name=nameof(ResumeGroup))]
        [ProducesResponseType(202)]
        public async Task<IActionResult> ResumeGroup(
            [FromRoute] string groupId,
            [FromQuery] IList<string> topics)
        {
            await this.adminProducer.ProduceAsync(
                new ResumeConsumersByGroup
                {
                    GroupId = groupId,
                    Topics = topics,
                });

            return this.Accepted();
        }
    }
}
