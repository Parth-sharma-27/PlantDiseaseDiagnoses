namespace PlantDiaganoseDisease.IServices
{
    public interface IJwtAuthService
    {
        public Task<string> GenerateJwtToken(int userId, string name, string role);
    }
}
