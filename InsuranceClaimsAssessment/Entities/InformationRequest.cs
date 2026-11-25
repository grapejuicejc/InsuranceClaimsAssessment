using System.ComponentModel.DataAnnotations.Schema;

namespace InsuranceClaimsAssessment.Entities
{
    public class InformationRequest
    {
        public int Id { get; set; }

        public int ClaimId { get; set; }
        public Claim Claim { get; set; } = null!;

        public required string Message { get; set; }

        public int RequestedByUserId { get; set; }

        [ForeignKey(nameof(RequestedByUserId))]
        public User RequestedBy { get; set; } = null!;

        // UTC
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

        public string? ResponseText { get; set; }

        // UTC. Null means the request is unanswered.
        public DateTime? RespondedAt { get; set; }
    }
}
