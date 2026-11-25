using InsuranceClaimsAssessment.Contracts;
using InsuranceClaimsAssessment.Data;
using InsuranceClaimsAssessment.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace InsuranceClaimsAssessment.Services
{
    public class StaffClaimService
    {
        private readonly ClaimsDbContext _db;

        public StaffClaimService(ClaimsDbContext db)
        {
            _db = db;
        }

        public async Task<List<StaffClaimItem>> GetAsync(int staffId, ClaimStatus? status, bool mine)
        {
            var claims = _db.Claims.AsQueryable();

            if (status is not null)
            {
                claims = claims.Where(c => c.Status == status);
            }

            if (mine)
            {
                claims = claims.Where(c => c.AssignedStaffId == staffId);
            }

            // Oldest first, so nobody's claim is left behind
            return await ToItems(claims.OrderBy(c => c.CreatedAt)).ToListAsync();
        }

        public async Task<ServiceResult<StaffClaimItem>> PickUpAsync(int staffId, int claimId)
        {
            var claim = await _db.Claims.SingleOrDefaultAsync(c => c.Id == claimId);

            if (claim is null)
            {
                return Failure(ServiceError.NotFound, "Claim not found.");
            }

            if (claim.Status != ClaimStatus.Submitted)
            {
                return Failure(ServiceError.Conflict, "This claim has already been picked up.");
            }

            claim.AssignedStaffId = staffId;
            ChangeStatus(claim, ClaimStatus.UnderReview, staffId);

            return await SaveAsync(claim);
        }

        public async Task<ServiceResult<StaffClaimItem>> SettleAsync(
            int staffId, int claimId, SettleClaimRequest request)
        {
            var amount = request.Amount!.Value;

            // The database keeps two decimals, so do not let it change the amount
            if (decimal.Round(amount, 2) != amount)
            {
                return Failure(ServiceError.Invalid, "The amount can have at most two decimals.");
            }

            return await DecideAsync(staffId, claimId, DecisionOutcome.Settled, request.Reason, amount);
        }

        public async Task<ServiceResult<StaffClaimItem>> RejectAsync(
            int staffId, int claimId, RejectClaimRequest request)
        {
            return await DecideAsync(staffId, claimId, DecisionOutcome.Rejected, request.Reason, null);
        }

        private async Task<ServiceResult<StaffClaimItem>> DecideAsync(
            int staffId, int claimId, DecisionOutcome outcome, string reason, decimal? settledAmount)
        {
            var claim = await _db.Claims.SingleOrDefaultAsync(c => c.Id == claimId);

            if (claim is null)
            {
                return Failure(ServiceError.NotFound, "Claim not found.");
            }

            if (claim.Status != ClaimStatus.UnderReview)
            {
                return Failure(ServiceError.Conflict, "Only a claim under review can be decided.");
            }

            if (claim.AssignedStaffId != staffId)
            {
                return Failure(ServiceError.Forbidden, "Only the assigned staff member can decide this claim.");
            }

            var newStatus = outcome == DecisionOutcome.Settled ? ClaimStatus.Settled : ClaimStatus.Rejected;
            ChangeStatus(claim, newStatus, staffId);

            claim.Decision = new ClaimDecision
            {
                Outcome = outcome,
                Reason = reason,
                SettledAmount = settledAmount,
                DecidedByUserId = staffId,
                DecidedAt = claim.UpdatedAt
            };

            return await SaveAsync(claim);
        }

        private static void ChangeStatus(Claim claim, ClaimStatus status, int staffId)
        {
            var now = DateTime.UtcNow;

            claim.Status = status;
            claim.UpdatedAt = now;

            claim.StatusHistory.Add(new ClaimStatusHistory
            {
                Status = status,
                ChangedAt = now,
                ChangedByUserId = staffId
            });
        }

        private async Task<ServiceResult<StaffClaimItem>> SaveAsync(Claim claim)
        {
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException exception) when (IsConflict(exception))
            {
                return Failure(ServiceError.Conflict, "Someone else changed this claim. Reload and try again.");
            }

            var item = await ToItems(_db.Claims.Where(c => c.Id == claim.Id)).SingleAsync();

            return ServiceResult<StaffClaimItem>.Success(item);
        }

        // A stale RowVersion, or a second decision refused by the unique index
        private static bool IsConflict(DbUpdateException exception)
        {
            return exception is DbUpdateConcurrencyException
                || exception.InnerException is SqlException { Number: 2601 or 2627 };
        }

        private static ServiceResult<StaffClaimItem> Failure(ServiceError error, string message)
        {
            return ServiceResult<StaffClaimItem>.Failure(error, message);
        }

        private static IQueryable<StaffClaimItem> ToItems(IQueryable<Claim> claims)
        {
            return claims.Select(c => new StaffClaimItem
            {
                Id = c.Id,
                ReferenceNumber = c.ReferenceNumber,
                PolicyNumber = c.Policy.PolicyNumber,
                CoverType = c.Policy.CoverType.ToString(),
                IncidentDate = DateOnly.FromDateTime(c.IncidentDate),
                Description = c.Description,
                EstimatedAmount = c.EstimatedAmount,
                Status = c.Status.ToString(),
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                ClaimantName = c.Claimant.DisplayName,
                AssignedStaffId = c.AssignedStaffId,
                AssignedStaffName = c.AssignedStaff != null ? c.AssignedStaff.DisplayName : null,
                WaitingForClaimant = c.Status == ClaimStatus.UnderReview
                    && c.InformationRequests.Any(r => r.RespondedAt == null)
            });
        }
    }
}
