namespace KafkaFlow.Sample.SchemaRegistryProducer
{
    using System.Runtime.Serialization;

    // KafkaFlow uses the DataContract Name property to find the schema in Schema Registry API
    // The format is subject_name@version
    [DataContract(Name = "test-topic-schema-registry-value@1")]
    public class TestMessageV1
    {
        public string Text { get; set; }
    }

        /* This schema must be registered in Schema Registry with subject name test-topic-schema-registry-value
{
  "doc": "Sample schema to help you get started.",
  "fields": [
    {
      "name": "Text",
      "type": "string"
    }
  ],
  "name": "value_test_topic_schema_registry",
  "namespace": "com.mycorp.mynamespace",
  "type": "record"
}
     */
}
