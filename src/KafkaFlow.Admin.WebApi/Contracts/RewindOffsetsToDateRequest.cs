namespace KafkaFlow.Admin.WebApi.Contracts
{
    using System;

    public class RewindOffsetsToDateRequest
    {
        public DateTime Date { get; set; }
    }
}
