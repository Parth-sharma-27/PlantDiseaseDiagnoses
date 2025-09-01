namespace PlantDiaganoseDisease.Models.RequestModels
{
    public class TreatmentDosageReq
    {
        public string? TreatmentType { get; set; }
        public string? ProductName { get; set; }
        public double? FarmArea { get; set; } 
        public string? DilutionRatio { get; set; }
        public double ProductConcentration { get; set; } 
        public double WaterAvailable { get; set; }
        public string PreferredLanguage { get; set; } = "auto";
    }
}
