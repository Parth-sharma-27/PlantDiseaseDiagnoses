namespace PlantDiaganoseDisease.Models.RequestModels
{
    public class LoginReqModel
    {
        public string? MobileOrEmail { get; set; }
        public string? Password { get; set; }
        public string? VerificationCode { get; set; }
        public bool IsResendOTP { get; set; } = false;
    }
}
