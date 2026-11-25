using InsuranceClaimsAssessment.Contracts;
using InsuranceClaimsAssessment.Entities;
using InsuranceClaimsAssessment.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceClaimsAssessment.Controllers
{
    [Route("api")]
    [Authorize(Roles = nameof(UserRole.ClaimsStaff))]
    public class StaffClaimsController : ApiControllerBase
    {
        private readonly StaffClaimService _staffClaimService;

        public StaffClaimsController(StaffClaimService staffClaimService)
        {
            _staffClaimService = staffClaimService;
        }

        // For example ?status=Submitted for incoming claims, or ?mine=true for own workload
        [HttpGet("staff/claims")]
        public async Task<ActionResult<List<StaffClaimItem>>> Get(ClaimStatus? status, bool mine = false)
        {
            return await _staffClaimService.GetAsync(UserId, status, mine);
        }

        [HttpPost("claims/{id:int}/pickup")]
        public async Task<ActionResult<StaffClaimItem>> PickUp(int id)
        {
            return ToResponse(await _staffClaimService.PickUpAsync(UserId, id));
        }

        [HttpPost("claims/{id:int}/settle")]
        public async Task<ActionResult<StaffClaimItem>> Settle(int id, SettleClaimRequest request)
        {
            return ToResponse(await _staffClaimService.SettleAsync(UserId, id, request));
        }

        [HttpPost("claims/{id:int}/reject")]
        public async Task<ActionResult<StaffClaimItem>> Reject(int id, RejectClaimRequest request)
        {
            return ToResponse(await _staffClaimService.RejectAsync(UserId, id, request));
        }

        private ActionResult<StaffClaimItem> ToResponse(ServiceResult<StaffClaimItem> result)
        {
            if (result.Error is not null)
            {
                return ToProblem(result.Error.Value, result.Message);
            }

            return result.Value!;
        }
    }
}
