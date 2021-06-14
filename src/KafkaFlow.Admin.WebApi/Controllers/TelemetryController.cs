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
    [Route("kafka-flow/telemetry")]
    [ApiController]
    public class TelemetryController : ControllerBase
    {
        private readonly IConsumerAccessor consumers;
        private readonly ITelemetryStorage storage;

        /// <summary>
        /// Initializes a new instance of the <see cref="TelemetryController"/> class.
        /// </summary>
        /// <param name="consumers">The accessor class that provides access to the consumers</param>
        /// <param name="storage">The cache interface to get metric data</param>
        public TelemetryController(IConsumerAccessor consumers, ITelemetryStorage storage)
        {
            this.consumers = consumers;
            this.storage = storage;
        }

        /// <summary>
        /// Get telemetry data from all the consumer groups
        /// </summary>
        /// <returns>A list of consumer groups</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TelemetryGroupResponse>), 200)]
        public IActionResult Get()
        {
            return this.Ok(
                this.consumers.All
                    .GroupBy(x => x.GroupId)
                    .Select(
                        x => new TelemetryGroupResponse
                        {
                            GroupId = x.First().GroupId,
                            Consumers = x.Select(y => y.Adapt(this.storage)),
                        }));
        }
    }
}
