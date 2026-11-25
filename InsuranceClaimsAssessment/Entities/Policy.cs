namespace InsuranceClaimsAssessment.Entities
{
    public class Policy
    {
        public int Id { get; set; }

        public required string PolicyNumber { get; set; }

        public int ClaimantId { get; set; }
        public User Claimant { get; set; } = null!;

        public required CoverType CoverType { get; set; }

        public required DateTime StartDate { get; set; }

        public required DateTime EndDate { get; set; }

        public ICollection<Claim> Claims { get; set; } = new List<Claim>();
    }
}
