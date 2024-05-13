using DataAccess.DTO;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text.Json;

namespace DataAccess
{
    public class WeatherAccess : IWeatherAccess
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger _logger;
        public WeatherAccess(IHttpClientFactory httpClientFactory, ILogger<WeatherAccess> logger)
        {
            _clientFactory = httpClientFactory;
            _logger = logger;
        }
        public T CallWeatherApi<T>(string method, string argument)
        {
            HttpRequestMessage request;
            //Switch used for different types of calls to Weather API.
            switch (method)
            {
                case "current":
                    request = CreateHTTPRequest(HttpMethod.Get, "current.json", argument);
                    break;
                case "history":
                    request = CreateHTTPRequest(HttpMethod.Get, "history.json", argument);
                    break;
                default:
                    throw new NotImplementedException();
            }
            //Create the named client for Weather API
            var client = _clientFactory.CreateClient("WeatherHttpClient");
            var response = client.Send(request);
            try
            {
                //Ensure that the call was success. An if statement can be used that checks respone.IsSuccessStatusCode and retry in case, but this could be considered overengineering for such a simple case.
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Encountered error while calling Weather API. {ex.Message}");
                throw;
            }
            //Process the result and deserialize this into the provided class object
            using var reader = new StreamReader(response.Content.ReadAsStream());
            return JsonSerializer.Deserialize<T>(reader.ReadToEnd());
        }

        //Simple method to create an HTTP request to avoid duplicating code above.
        private HttpRequestMessage CreateHTTPRequest(HttpMethod httpMethod, string path, string argument) => new HttpRequestMessage(HttpMethod.Get, $"{path}?q={argument}");
    }
}