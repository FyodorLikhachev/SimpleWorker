using Microsoft.Extensions.Configuration;
using System;

namespace TestApplication
{
    internal static class Extensions
    {
        public static int GetIntConfig(this IConfiguration config, string configName) 
        {
            var configValue = config.GetStringConfig(configName);

            if (!int.TryParse(configValue, out int intSetting))
                throw new ArgumentException(
                    $"Config: '{configName}' with value '{configValue}' isn't a valid integer");

            return intSetting;
        }

        public static string GetStringConfig(this IConfiguration config, string configName)
        {
            var configValue = config.GetValue<string>(configName)
                ?? throw new ArgumentException($"Config '{configName}' isn't specified");

            return configValue;
        }

        public static Uri GetUriConfig(this IConfiguration config, string configName)
        {
            var configValue = config.GetStringConfig(configName);

            if (!Uri.TryCreate(configValue, UriKind.Absolute, out var uri))
                throw new ArgumentException(
                    $"Config: '{configName}' with value '{configValue}' isn't a valid uri");

            return uri;
        }
    }
}
