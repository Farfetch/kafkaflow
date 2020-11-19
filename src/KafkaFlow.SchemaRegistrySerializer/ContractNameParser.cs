namespace KafkaFlow.SchemaRegistrySerializer
{
    using System;

    internal static class ContractNameParser
    {
        public static bool TryGetSchemaData(string contractName, out string subject, out int version)
        {
            subject = null;
            version = 0;

            if (string.IsNullOrWhiteSpace(contractName))
            {
                return false;
            }

            var values = contractName.Split('@');

            if (values.Length != 2 || !int.TryParse(values[1], out version))
            {
                return false;
            }

            subject = values[0];

            return true;
        }

        public static (string subject, int version) GetSchemaData(string contractName)
        {
            if (TryGetSchemaData(contractName, out var subject, out var version))
                return (subject, version);

            throw new ArgumentException(
                $"The contract name must be formatted as 'schema_subject@version_number' and it is '{contractName}'",
                nameof(contractName));
        }

        public static string GetContractName(string subject, int version) => $"{subject}@{version}";
    }
}
