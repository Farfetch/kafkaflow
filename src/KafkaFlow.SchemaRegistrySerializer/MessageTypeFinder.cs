namespace KafkaFlow.SchemaRegistrySerializer
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;

    //TODO: create unit tests here
    internal class MessageTypeFinder : IMessageTypeFinder
    {
        private readonly ConcurrentDictionary<(string, int), Type> typesCache =
            new ConcurrentDictionary<(string, int), Type>();

        private readonly IEnumerable<Type> availableTypes;

        public MessageTypeFinder(IEnumerable<Type> availableTypes)
        {
            this.availableTypes = availableTypes;
        }

        public Type Find(string subject, int version)
        {
            return this.typesCache.GetOrAdd(
                (subject, version),
                _ => this.TryGetExactTypeVersion(subject, version) ?? this.TryGetClosestVersion(subject));
        }

        private Type TryGetExactTypeVersion(string subject, int version)
        {
            var contractName = ContractNameParser.GetContractName(subject, version);

            return this.availableTypes.FirstOrDefault(
                type =>
                {
                    var contract = type.GetCustomAttribute<DataContractAttribute>();

                    return contractName == contract?.Name;
                });
        }

        private Type TryGetClosestVersion(string subject)
        {
            return this.availableTypes
                .Select(
                    type =>
                    {
                        var contract = type.GetCustomAttribute<DataContractAttribute>();

                        if (
                            !ContractNameParser.TryGetSchemaData(contract?.Name, out var contractSubject, out var version) ||
                            contractSubject != subject)
                        {
                            return null;
                        }

                        return new { type, subject, version };
                    })
                .Where(x => x != null)
                .OrderByDescending(x => x.version)
                .Select(x => x.type)
                .FirstOrDefault();
        }
    }
}
