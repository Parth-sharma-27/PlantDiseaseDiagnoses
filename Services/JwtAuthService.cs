using Microsoft.IdentityModel.Tokens;
using PlantDiaganoseDisease.IServices;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PlantDiaganoseDisease.Services
{
    public class JwtAuthService : IJwtAuthService
    {
        private readonly IConfiguration _configuration;

        public JwtAuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<string> GenerateJwtToken(int userId, string name, string role)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(role))
            {
                return "Invalid parameters"; // Return a clear error message for invalid input
            }

            try
            {
                var claims = new[]
                {
                new Claim(ClaimTypes.Name, name),
                new Claim("userId", userId.ToString()),
                new Claim(ClaimTypes.Role, role)
            };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    //Expires = DateTime.UtcNow.AddMinutes(10),
                    Expires = DateTime.UtcNow.AddDays(10),
                    Issuer = _configuration["Jwt:Issuer"],
                    Audience = _configuration["Jwt:Audience"],
                    SigningCredentials = credentials
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                return await Task.FromResult(tokenString);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }

}
