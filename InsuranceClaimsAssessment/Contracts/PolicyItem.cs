namespace InsuranceClaimsAssessment.Contracts
{
    public class PolicyItem
    {
        public required int Id { get; set; }

        public required string PolicyNumber { get; set; }

        public required string CoverType { get; set; }

        public required DateOnly StartDate { get; set; }

        public required DateOnly EndDate { get; set; }
    }
}
