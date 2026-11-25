using InsuranceClaimsAssessment.Contracts;
using InsuranceClaimsAssessment.Entities;
using InsuranceClaimsAssessment.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceClaimsAssessment.Controllers
{
    [ApiController]
    [Route("api/claims")]
    [Authorize]
    public class ClaimsController : ControllerBase
    {
        private readonly ClaimService _claimService;

        public ClaimsController(ClaimService claimService)
        {
            _claimService = claimService;
        }

        // From the token, never from the request
        private int UserId => int.Parse(User.FindFirst("sub")!.Value);

        [HttpPost]
        [Authorize(Roles = nameof(UserRole.Claimant))]
        public async Task<ActionResult<ClaimResponse>> Create(CreateClaimRequest request)
        {
            var result = await _claimService.CreateAsync(UserId, request);

            if (result.Error is not null)
            {
                return ToProblem(result.Error.Value, result.Message);
            }

            return Created($"/api/claims/{result.Value!.Id}", result.Value);
        }

        private ObjectResult ToProblem(ServiceError error, string? message)
        {
            var statusCode = error switch
            {
                ServiceError.NotFound => StatusCodes.Status404NotFound,
                ServiceError.Invalid => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };

            return Problem(title: message, statusCode: statusCode);
        }
    }
}
