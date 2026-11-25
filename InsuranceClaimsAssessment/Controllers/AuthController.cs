using InsuranceClaimsAssessment.Contracts;
using InsuranceClaimsAssessment.Data;
using InsuranceClaimsAssessment.Entities;
using InsuranceClaimsAssessment.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InsuranceClaimsAssessment.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly ClaimsDbContext _db;
        private readonly TokenService _tokenService;
        private readonly IConfiguration _configuration;

        public AuthController(ClaimsDbContext db, TokenService tokenService, IConfiguration configuration)
        {
            _db = db;
            _tokenService = tokenService;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
        {
            var user = await _db.Users.SingleOrDefaultAsync(u => u.Email == request.Email);

            // Same answer for an unknown email and a wrong password
            if (user is null || !IsPasswordCorrect(user, request.Password))
            {
                return Problem(title: "Invalid email or password.", statusCode: StatusCodes.Status401Unauthorized);
            }

            var expiresAt = DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("Jwt:ExpiryMinutes"));

            return new LoginResponse
            {
                Token = _tokenService.CreateToken(user, expiresAt),
                ExpiresAt = expiresAt,
                DisplayName = user.DisplayName,
                Role = user.Role.ToString()
            };
        }

        private static bool IsPasswordCorrect(User user, string password)
        {
            var result = new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, password);

            return result != PasswordVerificationResult.Failed;
        }
    }
}
