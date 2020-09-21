namespace KafkaFlow.UnitTests.Serializers.Resources
{
    using System;
    using ProtoBuf;

    [ProtoContract]
    public class PersonFromProtobufNet
    {
        [ProtoMember(1)]
        public int Id { get; set; }
        
        [ProtoMember(2)]
        public string Name { get; set; }
        
        [ProtoMember((3))]
        public string Email { get; set; }

        protected bool Equals(PersonFromProtobufNet other)
        {
            return Id == other.Id && Name == other.Name && Email == other.Email;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PersonFromProtobufNet) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, Email);
        }
    }
}