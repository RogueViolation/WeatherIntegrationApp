using DataAccess;
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
            try
            {
                //Return Ok result with the JSON data
                return Ok(_processWrapper.GenerateWeatherFormatJSON(locations));
            }
            //In case something fails, log it and return HTTP 500
            catch (Exception e)
            {
                _logger.LogError($"Encountered error while processing request GetCurrentWeather. {e.Message}");
                return StatusCode(500);
            }
        }

        //Endpoint to create a JSON file for multiple cities
        [HttpPost("CreateWeatherFile")]
        public ActionResult Post([FromQuery] List<string> locations)
        {
            try
            {
                return Ok(_processWrapper.GenerateWeatherJSONFile(locations));
            }
            catch (Exception e)
            {
                _logger.LogError($"Encountered error while processing request CreateWeatherFile. {e.Message}");
                return StatusCode(500);
            }
        }

        //Hardcoded example that fulfills the logic that is required by the task.
        [HttpGet("Example")]
        public ActionResult PostExample()
        {
            try
            {
                //This is hardcoded because the task emphasizes this.
                return Ok(_processWrapper.GenerateWeatherJSONFile(new List<string> { "Vilnius", "Kaunas", "Klaipeda" }));
            }
            catch (Exception e)
            {
                _logger.LogError($"Encountered error while processing request Example. {e.Message}");
                return StatusCode(500);
            }
        }
    }
}
