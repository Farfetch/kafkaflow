using System.ComponentModel;
using System.Runtime.Serialization;

namespace SchemaRegistry;

[DataContract(Name = nameof(AvroConvertLogMessage), Namespace = "SchemaRegistry")]
public class AvroConvertLogMessage
{
    public string Message { get; set; }

    [DefaultValue(0)]
    public int Code { get; set; }
}
