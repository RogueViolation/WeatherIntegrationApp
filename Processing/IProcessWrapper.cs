using Processing.DataObjects;
using System.Text.Json;

namespace Utilities
{
    public interface IProcessWrapper
    {
        JsonDocument GenerateWeatherFormatJSON(List<string> locations);
        JsonDocument GenerateWeatherJSONFile(List<string> locations);
    }
}
