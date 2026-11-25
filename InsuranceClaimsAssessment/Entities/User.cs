namespace InsuranceClaimsAssessment.Entities
{
    public class User
    {
        public int Id { get; set; }

        public required string Email { get; set; }

        public required string PasswordHash { get; set; }

        public required string DisplayName { get; set; }

        public required UserRole Role { get; set; }

        // UTC
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Policy> Policies { get; set; } = new List<Policy>();

        public ICollection<Claim> Claims { get; set; } = new List<Claim>();
    }
}
