using PlantDiaganoseDisease.IServices;
using PlantDiaganoseDisease.Models.RequestModels;
using System.Diagnostics.Metrics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace PlantDiaganoseDisease.Services
{
    public class LocationService : ILocationService
    {
        private readonly HttpClient _httpClient;
        
        //private readonly HttpClient _httpClient;

        public LocationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Get all countries
        public async Task<IEnumerable<Country>> GetCountriesAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<CountriesNowResponse<List<Country>>>(
                "https://countriesnow.space/api/v0.1/countries"
            );

            return response?.Data ?? new List<Country>();
        }

        // Get states by country name
        public async Task<IEnumerable<State>> GetStatesByCountryAsync(string country)
        {
            var content = new StringContent(JsonSerializer.Serialize(new { country }), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("https://countriesnow.space/api/v0.1/countries/states", content);
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<CountriesNowResponse<CountryStatesData>>(json);
            return result?.Data?.States ?? new List<State>();
        }

        // Get cities by country + state name
        public async Task<IEnumerable<City>> GetCitiesByStateAsync(string country, string state)
        {
            var content = new StringContent(
                JsonSerializer.Serialize(new { country, state }),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync(
                "https://countriesnow.space/api/v0.1/countries/state/cities",
                content
            );

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            // Deserialize into List<string> since 'data' is array of city names
            var result = JsonSerializer.Deserialize<CountriesNowResponse<List<string>>>(json);

            return result?.Data.Select(c => new City { Name = c }) ?? new List<City>();
        }

    }
}
