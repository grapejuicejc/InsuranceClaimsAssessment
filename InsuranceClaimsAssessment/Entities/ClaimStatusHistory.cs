using System.ComponentModel.DataAnnotations.Schema;

namespace InsuranceClaimsAssessment.Entities
{
    public class ClaimStatusHistory
    {
        public int Id { get; set; }

        public int ClaimId { get; set; }
        public Claim Claim { get; set; } = null!;

        public required ClaimStatus Status { get; set; }

        // UTC
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

        public int ChangedByUserId { get; set; }

        [ForeignKey(nameof(ChangedByUserId))]
        public User ChangedBy { get; set; } = null!;
    }
}
