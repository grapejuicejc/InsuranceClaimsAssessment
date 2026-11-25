namespace InsuranceClaimsAssessment.Entities
{
    public class Claim
    {
        public int Id { get; set; }

        // Human-readable, for example CLM-2026-0001
        public required string ReferenceNumber { get; set; }

        public int ClaimantId { get; set; }
        public User Claimant { get; set; } = null!;

        // Null until a staff member picks up the claim
        public int? AssignedStaffId { get; set; }
        public User? AssignedStaff { get; set; }

        public int PolicyId { get; set; }
        public Policy Policy { get; set; } = null!;

        public required DateTime IncidentDate { get; set; }

        public required string Description { get; set; }

        // Null until estimated
        public decimal? EstimatedAmount { get; set; }

        public ClaimStatus Status { get; set; } = ClaimStatus.Submitted;

        // UTC
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // UTC
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Set by SQL Server on every save, so a stale update is refused
        public byte[] RowVersion { get; set; } = [];

        public ICollection<ClaimStatusHistory> StatusHistory { get; set; } = new List<ClaimStatusHistory>();

        public ICollection<InformationRequest> InformationRequests { get; set; } = new List<InformationRequest>();

        public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();

        public ClaimDecision? Decision { get; set; }
    }
}
