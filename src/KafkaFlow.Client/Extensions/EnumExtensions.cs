namespace KafkaFlow.Client.Extensions
{
    using System.ComponentModel;

    internal static class EnumExtensions
    {
        public static string GetDescription<T>(this T enumeration)
        {
            var attributes = typeof(T)
                .GetMember(enumeration.ToString())[0]
                .GetCustomAttributes(typeof(DescriptionAttribute), false);
            return ((DescriptionAttribute)attributes[0]).Description;
        }
    }
}
