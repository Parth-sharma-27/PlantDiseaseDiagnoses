namespace PlantDiaganoseDisease.Models.RequestModels
{
    public class FertilizerReq
    {
        public string? CropType { get; set; }
        public string? SoilType { get; set; }
        public double? FarmArea { get; set; }
        public string? GrowthStage { get; set; }
        public string? PreviousFertilizer { get; set; }
        public string? Language { get; set; }
    }
}
