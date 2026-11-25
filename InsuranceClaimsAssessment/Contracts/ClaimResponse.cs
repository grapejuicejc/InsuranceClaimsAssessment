namespace InsuranceClaimsAssessment.Contracts
{
    public class ClaimResponse
    {
        public required int Id { get; set; }

        public required string ReferenceNumber { get; set; }

        public required string PolicyNumber { get; set; }

        public required string CoverType { get; set; }

        public required DateOnly IncidentDate { get; set; }

        public required string Description { get; set; }

        public required decimal? EstimatedAmount { get; set; }

        public required string Status { get; set; }

        // UTC
        public required DateTime CreatedAt { get; set; }

        // UTC
        public required DateTime UpdatedAt { get; set; }
    }
}
