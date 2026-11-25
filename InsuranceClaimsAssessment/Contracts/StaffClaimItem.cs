namespace InsuranceClaimsAssessment.Contracts
{
    public class StaffClaimItem : ClaimResponse
    {
        public required string ClaimantName { get; set; }

        // Null until picked up
        public required int? AssignedStaffId { get; set; }

        public required string? AssignedStaffName { get; set; }

        public required bool WaitingForClaimant { get; set; }
    }
}
