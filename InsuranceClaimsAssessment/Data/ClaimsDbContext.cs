using InsuranceClaimsAssessment.Entities;
using Microsoft.EntityFrameworkCore;

namespace InsuranceClaimsAssessment.Data
{
    public class ClaimsDbContext : DbContext
    {
        public ClaimsDbContext(DbContextOptions<ClaimsDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();

        public DbSet<Policy> Policies => Set<Policy>();

        public DbSet<Claim> Claims => Set<Claim>();

        public DbSet<ClaimStatusHistory> ClaimStatusHistories => Set<ClaimStatusHistory>();

        public DbSet<InformationRequest> InformationRequests => Set<InformationRequest>();

        public DbSet<ClaimDecision> ClaimDecisions => Set<ClaimDecision>();

        public DbSet<Attachment> Attachments => Set<Attachment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(e =>
            {
                e.HasIndex(u => u.Email).IsUnique();
                e.Property(u => u.Email).HasMaxLength(256);
                e.Property(u => u.PasswordHash).HasMaxLength(512);
                e.Property(u => u.DisplayName).HasMaxLength(100);
                e.Property(u => u.Role).HasConversion<string>().HasMaxLength(30);
            });

            modelBuilder.Entity<Policy>(e =>
            {
                e.HasIndex(p => p.PolicyNumber).IsUnique();
                e.Property(p => p.PolicyNumber).HasMaxLength(50);
                e.Property(p => p.CoverType).HasConversion<string>().HasMaxLength(30);

                e.HasOne(p => p.Claimant)
                    .WithMany(u => u.Policies)
                    .HasForeignKey(p => p.ClaimantId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Claim>(e =>
            {
                e.HasIndex(c => c.ReferenceNumber).IsUnique();
                e.Property(c => c.ReferenceNumber).HasMaxLength(30);
                e.Property(c => c.Description).HasMaxLength(2000);
                e.Property(c => c.Status).HasConversion<string>().HasMaxLength(30);
                e.Property(c => c.EstimatedAmount).HasPrecision(18, 2);

                e.HasOne(c => c.Claimant)
                    .WithMany(u => u.Claims)
                    .HasForeignKey(c => c.ClaimantId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(c => c.AssignedStaff)
                    .WithMany()
                    .HasForeignKey(c => c.AssignedStaffId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(c => c.Policy)
                    .WithMany(p => p.Claims)
                    .HasForeignKey(c => c.PolicyId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ClaimStatusHistory>(e =>
            {
                e.Property(h => h.Status).HasConversion<string>().HasMaxLength(30);

                e.HasOne(h => h.Claim)
                    .WithMany(c => c.StatusHistory)
                    .HasForeignKey(h => h.ClaimId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(h => h.ChangedBy)
                    .WithMany()
                    .HasForeignKey(h => h.ChangedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<InformationRequest>(e =>
            {
                e.Property(r => r.Message).HasMaxLength(2000);
                e.Property(r => r.ResponseText).HasMaxLength(2000);

                e.HasOne(r => r.Claim)
                    .WithMany(c => c.InformationRequests)
                    .HasForeignKey(r => r.ClaimId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(r => r.RequestedBy)
                    .WithMany()
                    .HasForeignKey(r => r.RequestedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ClaimDecision>(e =>
            {
                e.Property(d => d.Outcome).HasConversion<string>().HasMaxLength(30);
                e.Property(d => d.Reason).HasMaxLength(2000);
                e.Property(d => d.SettledAmount).HasPrecision(18, 2);

                e.HasOne(d => d.Claim)
                    .WithOne(c => c.Decision)
                    .HasForeignKey<ClaimDecision>(d => d.ClaimId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(d => d.DecidedBy)
                    .WithMany()
                    .HasForeignKey(d => d.DecidedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Attachment>(e =>
            {
                e.Property(a => a.FileName).HasMaxLength(260);
                e.Property(a => a.ContentType).HasMaxLength(100);

                e.HasOne(a => a.Claim)
                    .WithMany(c => c.Attachments)
                    .HasForeignKey(a => a.ClaimId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(a => a.InformationRequest)
                    .WithMany()
                    .HasForeignKey(a => a.InformationRequestId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
