using Microsoft.Extensions.Configuration;

//Used to read secrets/configurations
namespace Utilities
{
    public class ConfigurationReader : IConfigurationReader
    {
        private readonly IConfiguration _config;

        public ConfigurationReader(IConfiguration config)
        {
            _config = config;
        }

        public string GetSection(string section) => _config.GetSection(section).Value ?? string.Empty;

    }
}
