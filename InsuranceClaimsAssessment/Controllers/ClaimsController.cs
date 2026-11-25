using InsuranceClaimsAssessment.Contracts;
using InsuranceClaimsAssessment.Entities;
using InsuranceClaimsAssessment.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceClaimsAssessment.Controllers
{
    [Route("api/claims")]
    [Authorize]
    public class ClaimsController : ApiControllerBase
    {
        private readonly ClaimService _claimService;

        public ClaimsController(ClaimService claimService)
        {
            _claimService = claimService;
        }

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

        [HttpGet]
        [Authorize(Roles = nameof(UserRole.Claimant))]
        public async Task<ActionResult<List<ClaimListItem>>> GetMine()
        {
            return await _claimService.GetForClaimantAsync(UserId);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = nameof(UserRole.Claimant))]
        public async Task<ActionResult<ClaimDetailsResponse>> GetMineById(int id)
        {
            var result = await _claimService.GetForClaimantByIdAsync(UserId, id);

            if (result.Error is not null)
            {
                return ToProblem(result.Error.Value, result.Message);
            }

            return result.Value!;
        }
    }
}
