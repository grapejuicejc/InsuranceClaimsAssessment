using System.Security.Cryptography;
using InsuranceClaimsAssessment.Contracts;
using InsuranceClaimsAssessment.Data;
using InsuranceClaimsAssessment.Entities;
using Microsoft.EntityFrameworkCore;

namespace InsuranceClaimsAssessment.Services
{
    public class ClaimService
    {
        // No 0, O, 1 or I, which are easy to misread
        private const string ReferenceCharacters = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

        private readonly ClaimsDbContext _db;

        public ClaimService(ClaimsDbContext db)
        {
            _db = db;
        }

        public async Task<ServiceResult<ClaimResponse>> CreateAsync(int claimantId, CreateClaimRequest request)
        {
            var policy = await _db.Policies
                .SingleOrDefaultAsync(p => p.Id == request.PolicyId && p.ClaimantId == claimantId);

            // Same answer for a missing policy and someone else's policy
            if (policy is null)
            {
                return ServiceResult<ClaimResponse>.Failure(ServiceError.NotFound, "Policy not found.");
            }

            var now = DateTime.UtcNow;
            var incidentDate = request.IncidentDate!.Value;

            // Use the time zone furthest ahead (UTC+14), so a claimant's "today" is never refused
            if (incidentDate > DateOnly.FromDateTime(now.AddHours(14)))
            {
                return ServiceResult<ClaimResponse>.Failure(
                    ServiceError.Invalid, "The incident date cannot be in the future.");
            }

            if (incidentDate < DateOnly.FromDateTime(policy.StartDate)
                || incidentDate > DateOnly.FromDateTime(policy.EndDate))
            {
                return ServiceResult<ClaimResponse>.Failure(
                    ServiceError.Invalid, "The incident date is outside the policy dates.");
            }

            var claim = new Claim
            {
                ReferenceNumber = CreateReferenceNumber(now),
                ClaimantId = claimantId,
                Policy = policy,
                IncidentDate = incidentDate.ToDateTime(TimeOnly.MinValue),
                Description = request.Description,
                EstimatedAmount = request.EstimatedAmount,
                Status = ClaimStatus.Submitted,
                CreatedAt = now,
                UpdatedAt = now
            };

            claim.StatusHistory.Add(new ClaimStatusHistory
            {
                Status = ClaimStatus.Submitted,
                ChangedAt = now,
                ChangedByUserId = claimantId
            });

            _db.Claims.Add(claim);
            await _db.SaveChangesAsync();

            return ServiceResult<ClaimResponse>.Success(ToResponse(claim));
        }

        public async Task<List<ClaimListItem>> GetForClaimantAsync(int claimantId)
        {
            return await _db.Claims
                .Where(c => c.ClaimantId == claimantId)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new ClaimListItem
                {
                    Id = c.Id,
                    ReferenceNumber = c.ReferenceNumber,
                    PolicyNumber = c.Policy.PolicyNumber,
                    CoverType = c.Policy.CoverType.ToString(),
                    IncidentDate = DateOnly.FromDateTime(c.IncidentDate),
                    Status = c.Status.ToString(),
                    WaitingForClaimant = c.Status == ClaimStatus.UnderReview
                        && c.InformationRequests.Any(r => r.RespondedAt == null),
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                })
                .ToListAsync();
        }

        public async Task<ServiceResult<ClaimDetailsResponse>> GetForClaimantByIdAsync(int claimantId, int claimId)
        {
            var claim = await _db.Claims
                .AsNoTracking()
                .AsSplitQuery()
                .Include(c => c.Policy)
                .Include(c => c.StatusHistory)
                .Include(c => c.InformationRequests)
                .Include(c => c.Decision)
                .SingleOrDefaultAsync(c => c.Id == claimId && c.ClaimantId == claimantId);

            // Same answer for a missing claim and someone else's claim
            if (claim is null)
            {
                return ServiceResult<ClaimDetailsResponse>.Failure(ServiceError.NotFound, "Claim not found.");
            }

            return ServiceResult<ClaimDetailsResponse>.Success(ToDetails(claim));
        }

        private static string CreateReferenceNumber(DateTime now)
        {
            return $"CLM-{now.Year}-{RandomNumberGenerator.GetString(ReferenceCharacters, 8)}";
        }

        private static ClaimResponse ToResponse(Claim claim)
        {
            return new ClaimResponse
            {
                Id = claim.Id,
                ReferenceNumber = claim.ReferenceNumber,
                PolicyNumber = claim.Policy.PolicyNumber,
                CoverType = claim.Policy.CoverType.ToString(),
                IncidentDate = DateOnly.FromDateTime(claim.IncidentDate),
                Description = claim.Description,
                EstimatedAmount = claim.EstimatedAmount,
                Status = claim.Status.ToString(),
                CreatedAt = claim.CreatedAt,
                UpdatedAt = claim.UpdatedAt
            };
        }

        private static ClaimDetailsResponse ToDetails(Claim claim)
        {
            return new ClaimDetailsResponse
            {
                Id = claim.Id,
                ReferenceNumber = claim.ReferenceNumber,
                PolicyNumber = claim.Policy.PolicyNumber,
                CoverType = claim.Policy.CoverType.ToString(),
                IncidentDate = DateOnly.FromDateTime(claim.IncidentDate),
                Description = claim.Description,
                EstimatedAmount = claim.EstimatedAmount,
                Status = claim.Status.ToString(),
                CreatedAt = claim.CreatedAt,
                UpdatedAt = claim.UpdatedAt,
                WaitingForClaimant = claim.Status == ClaimStatus.UnderReview
                    && claim.InformationRequests.Any(r => r.RespondedAt == null),
                History = claim.StatusHistory
                    .OrderBy(h => h.ChangedAt)
                    .Select(h => new ClaimHistoryItem
                    {
                        Status = h.Status.ToString(),
                        ChangedAt = h.ChangedAt
                    })
                    .ToList(),
                InformationRequests = claim.InformationRequests
                    .OrderBy(r => r.RequestedAt)
                    .Select(r => new InformationRequestItem
                    {
                        Id = r.Id,
                        Message = r.Message,
                        RequestedAt = r.RequestedAt,
                        ResponseText = r.ResponseText,
                        RespondedAt = r.RespondedAt
                    })
                    .ToList(),
                Decision = claim.Decision is null ? null : new ClaimDecisionItem
                {
                    Outcome = claim.Decision.Outcome.ToString(),
                    Reason = claim.Decision.Reason,
                    SettledAmount = claim.Decision.SettledAmount,
                    DecidedAt = claim.Decision.DecidedAt
                }
            };
        }
    }
}
