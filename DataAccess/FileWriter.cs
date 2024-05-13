using Microsoft.Extensions.Logging;
using System.Text.Json;
using Utility;

namespace DataAccess
{
    public class FileWriter : IFileWriter
    {
        private readonly ILogger _logger;
        private readonly IConfigurationReader _config;
        public FileWriter(ILogger<FileWriter> logger, IConfigurationReader config) 
        {
            _logger = logger;
            _config = config;
        }

        public string WriteJSONToSystem(JsonDocument json)
        {
            try {
                //Get section from secret.json that contains the path
                var path = _config.GetSection("Weather:Path");
                //Create a unique file name
                var filename = $"{Guid.NewGuid()}.json";
                //This code creates a directory if it does not exist. This line of code does nothing if the directory already exists.
                Directory.CreateDirectory(path);
                //Finally write the file locally.
                File.WriteAllText($"{path}{filename}", JsonSerializer.Serialize(json));

                //The file name could be used to check for all the files and perhapos download the one that we need if the there is a need for this.
                return filename;
            }
            catch 
            {
                _logger.LogError("Error writing JSON to file.");
                throw; 
            }
        }
    }
}
