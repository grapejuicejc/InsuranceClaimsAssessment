namespace InsuranceClaimsAssessment.Contracts
{
    public class LoginResponse
    {
        public required string Token { get; set; }

        // UTC
        public required DateTime ExpiresAt { get; set; }

        public required string DisplayName { get; set; }

        public required string Role { get; set; }
    }
}
