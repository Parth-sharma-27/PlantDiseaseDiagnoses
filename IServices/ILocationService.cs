using PlantDiaganoseDisease.Models.RequestModels;
using System.Diagnostics.Metrics;

namespace PlantDiaganoseDisease.IServices
{
    public interface ILocationService
    {
        Task<IEnumerable<Country>> GetCountriesAsync();
        Task<IEnumerable<State>> GetStatesByCountryAsync(string countryIso2);
        Task<IEnumerable<City>> GetCitiesByStateAsync(string countryIso2, string stateIso2);
    }
}
