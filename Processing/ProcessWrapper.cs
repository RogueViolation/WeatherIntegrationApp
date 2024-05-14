using DataAccess;
using DataAccess.DTO;
using Processing.DataObjects;
using System.Text.Json;

//This whole wrapper is used so that logic is isolated and the controller only calls this logic and depends on it.
namespace Utilities
{
    public class ProcessWrapper : IProcessWrapper
    {
        private readonly IWeatherAccess _weatherAccess;
        private readonly IFileWriter _writer;
        public ProcessWrapper(IWeatherAccess weatherAccess, IFileWriter writer) 
        {
            _weatherAccess = weatherAccess;
            _writer = writer;
        }
        public JsonDocument GenerateWeatherFormatJSON(List<string> locations)
        {
            //Essentially just returns the formatted JSON document from the data that gets fetched from API.
            return FetchWeatherFormatJSON(locations);
        }

        public JsonDocument GenerateWeatherJSONFile(List<string> locations)
        {
            //Creates a JSON document that contains some file info.
            return JsonSerializer.SerializeToDocument(new { filename = _writer.WriteJSONToSystem(FetchWeatherFormatJSON(locations)) });
        }

        //Decided to move logic out because it is used in multiple places.
        private JsonDocument FetchWeatherFormatJSON(List<string> locations) 
        {
            Dictionary<string, Weather> map = new Dictionary<string, Weather>();

            foreach (string location in locations)
            {
                //Get the weather data for current iteration of location list
                var data = _weatherAccess.CallWeatherApi<WeatherData>("current", location);

                //Create the required JSON structure.
                map.Add(
                        data.location.name,
                        new Weather
                        {
                            //This API returns around 40 different weather conditions which includes snow condition and sunny conditions. This would not make sense during the winter if it is snowing and we used the precipitation amount to show "Rain" when it is snowing.
                            Precipitation = data.current.condition.text,
                            Temperature = data.current.temp_c,
                            WindSpeed = $"{Math.Floor(data.current.wind_kph / 3.6)} m/s"
                        }
                    );
            }

            return JsonSerializer.SerializeToDocument(map);
        }
    }
}
