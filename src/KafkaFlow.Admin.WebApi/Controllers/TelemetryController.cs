namespace KafkaFlow.Admin.WebApi.Controllers
{
    using KafkaFlow.Admin.WebApi.Adapters;
    using KafkaFlow.Admin.WebApi.Contracts;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Telemetry controller
    /// </summary>
    [Route("kafkaflow/telemetry")]
    [ApiController]
    public class TelemetryController : ControllerBase
    {
        private readonly ITelemetryStorage storage;

        /// <summary>
        /// Initializes a new instance of the <see cref="TelemetryController"/> class.
        /// </summary>
        /// <param name="storage">The telemetry storage</param>
        public TelemetryController(ITelemetryStorage storage)
        {
            this.storage = storage;
        }

        /// <summary>
        /// Get telemetry data from all the consumer groups
        /// </summary>
        /// <returns>A telemetry response</returns>
        [HttpGet(Name=nameof(GetTelemetry))]
        [ProducesResponseType(typeof(TelemetryResponse), 200)]
        public IActionResult GetTelemetry()
        {
            var metrics = this.storage.Get();

            return this.Ok(metrics.Adapt());
        }
    }
}
