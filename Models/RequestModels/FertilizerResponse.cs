namespace PlantDiaganoseDisease.Models.RequestModels
{
    public class FertilizerSchedule
    {
        public string Stage { get; set; }
        public string Fertilizer { get; set; }
        public string Quantity { get; set; }
        public string Timing { get; set; }
    }

    public class FertilizerResponse
    {
        public string PrimaryFertilizer { get; set; }
        public string QuantityRequired { get; set; }
        public string ApplicationTime { get; set; }
        public string ApplicationMethod { get; set; }
        public List<FertilizerSchedule> ApplicationSchedule { get; set; }
        public List<string> ApplicationTips { get; set; }
    }

}
