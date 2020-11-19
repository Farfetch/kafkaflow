namespace KafkaFlow.Sample.SchemaRegistryConsumer
{
    using System.Runtime.Serialization;

    // KafkaFlow uses the DataContract Name property to find the schema in Schema Registry API
    // The format is subject_name@version
    [DataContract(Name = "test-topic-schema-registry-value@2")]
    public class TestMessageV2
    {
        public string Text { get; set; }

        public string Text1 { get; set; }

        /* This schema must be registered in Schema Registry with subject name test-topic-schema-registry-value
{
  "doc": "Sample schema to help you get started.",
  "fields": [
    {
      "name": "Text",
      "type": "string"
    },
    {
      "default": "Text1 default value",
      "name": "Text1",
      "type": "string"
    }
  ],
  "name": "value_test_topic_schema_registry",
  "namespace": "com.mycorp.mynamespace",
  "type": "record"
}
        */
    }
}
