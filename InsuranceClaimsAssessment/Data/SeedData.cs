using InsuranceClaimsAssessment.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InsuranceClaimsAssessment.Data
{
    // Demo users and policies for local development only
    public static class SeedData
    {
        private const string DemoPassword = "Password123!";

        public static async Task AddAsync(ClaimsDbContext db)
        {
            // Already seeded
            if (await db.Users.AnyAsync())
            {
                return;
            }

            var claimant1 = CreateUser("claimant1@example.com", "Claimant One", UserRole.Claimant);
            var claimant2 = CreateUser("claimant2@example.com", "Claimant Two", UserRole.Claimant);
            var staff1 = CreateUser("staff1@example.com", "Staff One", UserRole.ClaimsStaff);
            var staff2 = CreateUser("staff2@example.com", "Staff Two", UserRole.ClaimsStaff);

            db.Users.AddRange(claimant1, claimant2, staff1, staff2);

            db.Policies.AddRange(
                CreatePolicy("MTR-0001", claimant1, CoverType.Motor),
                CreatePolicy("PRP-0001", claimant1, CoverType.Property),
                CreatePolicy("MTR-0002", claimant2, CoverType.Motor));

            await db.SaveChangesAsync();
        }

        private static User CreateUser(string email, string displayName, UserRole role)
        {
            var user = new User
            {
                Email = email,
                PasswordHash = string.Empty,
                DisplayName = displayName,
                Role = role
            };

            user.PasswordHash = new PasswordHasher<User>().HashPassword(user, DemoPassword);

            return user;
        }

        private static Policy CreatePolicy(string policyNumber, User claimant, CoverType coverType)
        {
            var today = DateTime.UtcNow.Date;

            return new Policy
            {
                PolicyNumber = policyNumber,
                Claimant = claimant,
                CoverType = coverType,
                StartDate = today.AddYears(-1),
                EndDate = today.AddYears(1)
            };
        }
    }
}
