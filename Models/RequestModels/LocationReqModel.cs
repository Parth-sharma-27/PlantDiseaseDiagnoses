using System.Text.Json.Serialization;

namespace PlantDiaganoseDisease.Models.RequestModels
{
    
    public class LocationReqModel
    {
        public string Iso2 { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
    public class Country
    {
        [JsonPropertyName("country")]
        public string Name { get; set; }

        [JsonPropertyName("cities")]
        public List<string> Cities { get; set; }
    }


    public class State
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("state_code")]
        public string StateCode { get; set; }
    }

    public class City
    {
        public string Name { get; set; }
    }

    // API Response Models
    public class CountriesNowResponse<T>
    {
        [JsonPropertyName("error")]
        public bool Error { get; set; }

        [JsonPropertyName("msg")]
        public string Msg { get; set; }

        [JsonPropertyName("data")]
        public T Data { get; set; }
    }

    public class CountryStatesData
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("iso3")]
        public string Iso3 { get; set; }

        [JsonPropertyName("iso2")]
        public string Iso2 { get; set; }

        [JsonPropertyName("states")]
        public List<State> States { get; set; }
    }

    
}
