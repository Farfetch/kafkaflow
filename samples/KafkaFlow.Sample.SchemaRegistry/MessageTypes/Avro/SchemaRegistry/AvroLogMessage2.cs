// ------------------------------------------------------------------------------
// <auto-generated>
//    Generated by avrogen, version 1.11.0.0
//    Changes to this file may cause incorrect behavior and will be lost if code
//    is regenerated
// </auto-generated>
// ------------------------------------------------------------------------------
namespace SchemaRegistry
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using Avro;
	using Avro.Specific;
	
	/// <summary>
	/// A simple log message.
	/// </summary>
	public partial class AvroLogMessage2 : ISpecificRecord
	{
		public static Schema _SCHEMA = Avro.Schema.Parse("{\"type\":\"record\",\"name\":\"AvroLogMessage2\",\"doc\":\"A simple log message.\",\"namespac" +
				"e\":\"SchemaRegistry\",\"fields\":[{\"name\":\"Message\",\"type\":\"string\"}]}");

		private string _message;

		public virtual Schema Schema
		{
			get
			{
				return AvroLogMessage2._SCHEMA;
			}
		}
		public string Message
		{
			get
			{
				return _message;
			}
			set
			{
				_message = value;
			}
		}
		public virtual object Get(int fieldPos)
		{
			switch (fieldPos)
			{
			case 0: return this.Message;
			default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Get()");
			};
		}
		public virtual void Put(int fieldPos, object fieldValue)
		{
			switch (fieldPos)
			{
			case 0: this.Message = (System.String)fieldValue; break;
			default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
			};
		}
	}
}
