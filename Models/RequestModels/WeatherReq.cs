namespace PlantDiaganoseDisease.Models.RequestModels
{
    public class WeatherReq
    {
        public string? Temperature { get; set; }
        public string? Condition { get; set; }
        public string? Humidity { get; set; }
        public string? WindSpeed { get; set; }
        public string? Visibility { get; set; }
    }
}
