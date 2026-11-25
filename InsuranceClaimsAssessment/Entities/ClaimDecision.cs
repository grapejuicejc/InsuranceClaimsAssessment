using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace InsuranceClaimsAssessment.Entities
{
    public class ClaimDecision
    {
        public int Id { get; set; }

        // Unique: a claim has at most one decision
        public int ClaimId { get; set; }
        public Claim Claim { get; set; } = null!;

        public required DecisionOutcome Outcome { get; set; }

        public required string Reason { get; set; }

        // Null when rejected
        [Precision(18, 2)]
        public decimal? SettledAmount { get; set; }

        public int DecidedByUserId { get; set; }

        [ForeignKey(nameof(DecidedByUserId))]
        public User DecidedBy { get; set; } = null!;

        // UTC
        public DateTime DecidedAt { get; set; } = DateTime.UtcNow;
    }
}
