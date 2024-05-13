using System.Text.Json;

namespace DataAccess
{
    public interface IFileWriter
    {
        string WriteJSONToSystem(JsonDocument json);
    }
}
