namespace PlantDiaganoseDisease.Models.RequestModels
{
    public class IrrigationReq
    {
        public string? CropType { get; set; }
        public string? SoilType { get; set; }
        public double? FarmArea { get; set; }
        public string? IrrigationMethod { get; set; }
        public string? WaterSource { get; set; }
        public string? Language { get; set; } // "en", "hi", "ta" etc.
    }
    public class IrrigationSchedule
    {
        public string Stage { get; set; }
        public string Frequency { get; set; }
        public string WaterDepth { get; set; }
        public string Duration { get; set; }
    }

    public class IrrigationResponse
    {
        public string IrrigationFrequency { get; set; }
        public string WaterRequired { get; set; }
        public string Duration { get; set; }
        public string BestTime { get; set; }
        public List<IrrigationSchedule> IrrigationSchedule { get; set; }
        public List<string> IrrigationTips { get; set; }
    }
}
