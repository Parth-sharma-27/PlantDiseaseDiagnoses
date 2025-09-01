namespace PlantDiaganoseDisease.Models.RequestModels
{
    public class RecommendedProductReq
    {
        public string ProductName { get; set; }
        public string Dosage { get; set; }
    }
    public class TreatmentTypeResponse
    {
        public List<RecommendedProductReq> RecommendedProducts { get; set; }
        public string GeneralInstructions { get; set; }
    }
}
