using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlantDiaganoseDisease.IServices;


using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace PlantDiaganoseDisease.Controllers
{
   [ApiController]
    [Route("api/[controller]")]
    public class LocationController : ControllerBase
    {
        private readonly ILocationService _locationService;

        public LocationController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpGet("countries")]
        public async Task<IActionResult> GetCountries()
        {
            var countries = await _locationService.GetCountriesAsync();
            return Ok(countries);
        }

        [HttpGet("states/{countryIso2}")]
        public async Task<IActionResult> GetStates(string countryIso2)
        {
            var states = await _locationService.GetStatesByCountryAsync(countryIso2);
            return Ok(states);
        }

        [HttpGet("cities/{countryIso2}/{stateIso2}")]
        public async Task<IActionResult> GetCities(string countryIso2, string stateIso2)
        {
            var cities = await _locationService.GetCitiesByStateAsync(countryIso2, stateIso2);
            return Ok(cities);
        }
    }
}
