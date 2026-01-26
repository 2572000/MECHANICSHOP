using Asp.Versioning;
using MechanicShop.Contracts.Response;
using MechanicShop.Infrastructure.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace MechanicShop.API.Controllers
{
    [Route("api/settings")]
    [ApiVersionNeutral]
    public class SettingsController(IOptionsMonitor<AppSettings> options) : ApiController
    {
        private readonly IOptionsMonitor<AppSettings> _settings = options;

        [HttpGet("operating-hours")]
        [ProducesResponseType(typeof(OperatingHoursResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Gets the application's operating hours.")]
        [EndpointDescription("Returns the current configured opening and closing times.")]
        [EndpointName("GetOperatingHours")]
        public IActionResult GetOperatingHours() 
        {
            return Ok(new OperatingHoursResponse(_settings.CurrentValue.OpeningTime, _settings.CurrentValue.ClosingTime));
        }
    }
}
