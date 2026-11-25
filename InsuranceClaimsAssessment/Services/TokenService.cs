using InsuranceClaimsAssessment.Entities;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace InsuranceClaimsAssessment.Services
{
    public class TokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreateToken(User user, DateTime expiresAt)
        {
            var key = new SymmetricSecurityKey(Convert.FromBase64String(_configuration["Jwt:Key"]!));

            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                Expires = expiresAt,
                Claims = new Dictionary<string, object>
                {
                    ["sub"] = user.Id.ToString(),
                    ["role"] = user.Role.ToString()
                },
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            };

            return new JsonWebTokenHandler().CreateToken(descriptor);
        }
    }
}
