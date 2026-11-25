using InsuranceClaimsAssessment.Contracts;
using InsuranceClaimsAssessment.Entities;
using InsuranceClaimsAssessment.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceClaimsAssessment.Controllers
{
    [Route("api/policies")]
    [Authorize(Roles = nameof(UserRole.Claimant))]
    public class PoliciesController : ApiControllerBase
    {
        private readonly ClaimService _claimService;

        public PoliciesController(ClaimService claimService)
        {
            _claimService = claimService;
        }

        [HttpGet]
        public async Task<ActionResult<List<PolicyItem>>> GetMine()
        {
            return await _claimService.GetPoliciesForClaimantAsync(UserId);
        }
    }
}
