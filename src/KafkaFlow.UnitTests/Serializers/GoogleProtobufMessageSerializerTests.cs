namespace KafkaFlow.UnitTests.Serializers
{
    using Google.Protobuf.Examples.AddressBook;
    using KafkaFlow.Serializer.GoogleProtobuf;
    using KafkaFlow.UnitTests.Serializers.Resources;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class GoogleProtobufMessageSerializerTests
    {
        private GoogleProtobufMessageSerializer serializer = new GoogleProtobufMessageSerializer();
        
        [TestMethod]
        public void SerializeAndDeserialize_WhenTheClassIsFromGoogleCompiler_ShouldProduceEqualsObjects()
        {
            object msg1 = CreateProtobufGoogleObject();
            
            // Act - 1
            var byteArray = serializer.Serialize(msg1);
            
            // Assert
            Assert.IsNotNull(byteArray);

            object msg2 = serializer.Deserialize(byteArray, typeof(Person));
            
            // Assert
            Assert.IsNotNull(msg2);
            Assert.AreNotSame(msg1, msg2);
            Assert.AreEqual(msg1, msg2);
        }
        
        [TestMethod]
        public void SerializeAndDeserialize_WhenTheClassIsFromProtobufNet_ShouldProduceEqualsObjects()
        {
            object msg1 = CreateProtobufNetObject();
            
            // Act - 1
            var byteArray = serializer.Serialize(msg1);
            
            // Assert
            Assert.IsNotNull(byteArray);

            object msg2 = serializer.Deserialize(byteArray, typeof(PersonFromProtobufNet));
            
            // Assert
            Assert.IsNotNull(msg2);
            Assert.AreNotSame(msg1, msg2);
            Assert.AreEqual(msg1, msg2);
        }

        private static object CreateProtobufGoogleObject()
        {
            /* The class Person was created using the addressbook.proto and the generated class Addressbook.cs
             using the compiler provided by Google, following the instructions from here: https://developers.google.com/protocol-buffers/docs/csharptutorial
             */
            object john = new Person
            {
                Id = 1234,
                Name = "John Doe",
                Email = "jdoe@example.com",
                Phones = { new Person.Types.PhoneNumber { Number = "555-4321", Type = Person.Types.PhoneType.Home } }
            };

            return john;
        }

        private static object CreateProtobufNetObject()
        {
            object jane = new PersonFromProtobufNet
            {
                Id = 321789,
                Name = "Jane Doe",
                Email = "jane.doe@example.com",
            };

            return jane;
        }
        
    }
}