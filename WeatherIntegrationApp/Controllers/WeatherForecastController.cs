using DataAccess;
using DataAccess.DTO;
using Microsoft.AspNetCore.Mvc;
using Utilities;

namespace WeatherIntegrationApp.Controllers
{
    //I think using v1 right off the bat is a good practice.
    [ApiController]
    [Route("v1/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IProcessWrapper _processWrapper;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IProcessWrapper processWrapper)
        {
            //The controller does not inject anything other than logger and processWrapper to avoid useless dependencies
            _processWrapper = processWrapper;
            _logger = logger;
        }

        //Endpoint for getting a JSON response for multiple cities
        [HttpGet("GetCurrentWeather")]
        public ActionResult Get([FromQuery]List<string> locations)
        {
            _logger.LogInformation($"Received GetCurrentWeather request from {HttpContext.Connection.RemoteIpAddress}.");
            try
            {
                if (locations.Count == 0)
                {
                    return StatusCode(400, ErrorMessage.CreateErrorObject("Locations list is empty", Empty));
                }
                //Return Ok result with the JSON data
                return Ok(_processWrapper.GenerateWeatherFormatJSON(locations));
            }
            //In case something fails, log it and return HTTP 500
            catch (Exception e)
            {
                _logger.LogError($"Encountered error while processing request GetCurrentWeather. {e.Message}");
                return StatusCode(500, ErrorMessage.CreateErrorObject(e.Message, locations));
            }
        }

        //Endpoint to create a JSON file for multiple cities
        [HttpPost("CreateWeatherFile")]
        public ActionResult Post([FromQuery] List<string> locations)
        {
            _logger.LogInformation($"Received CreateWeatherFile request from {HttpContext.Connection.RemoteIpAddress}.");
            try
            {
                if (locations.Count == 0)
                {
                    return StatusCode(400, ErrorMessage.CreateErrorObject("Locations list is empty.", Empty));
                }
                return Ok(_processWrapper.GenerateWeatherJSONFile(locations));
            }
            catch (Exception e)
            {
                _logger.LogError($"Encountered error while processing request CreateWeatherFile. {e.Message}");
                return StatusCode(500, ErrorMessage.CreateErrorObject(e.Message, locations));
            }
        }

        //Hardcoded example that fulfills the logic that is required by the task.
        [HttpGet("Example")]
        public ActionResult PostExample()
        {
            _logger.LogInformation($"Received Example request from {HttpContext.Connection.RemoteIpAddress}.");
            try
            {
                //This is hardcoded because the task emphasizes this.
                return Ok(_processWrapper.GenerateWeatherJSONFile(new List<string> { "Vilnius", "Kaunas", "Klaipeda" }));
            }
            catch (Exception e)
            {
                _logger.LogError($"Encountered error while processing request Example. {e.Message}");
                return StatusCode(500, ErrorMessage.CreateErrorObject(e.Message, new List<string> { "Vilnius", "Kaunas", "Klaipeda" }));
            }
        }
    }

    class ErrorMessage
    {
        public string Message { get; set; }
        public DateTime Time { get; set; }
        public object Arguments { get; set; }

        public static ErrorMessage CreateErrorObject(string message, object args) 
        { 
            return new ErrorMessage { Message = message, Time = DateTime.Now, Arguments = args};
        }
    }
}
