using System.ComponentModel.DataAnnotations;

namespace InsuranceClaimsAssessment.Contracts
{
    public class CreateClaimRequest
    {
        [Range(1, int.MaxValue)]
        public int PolicyId { get; set; }

        // A calendar date, for example 2026-11-20
        [Required]
        public DateOnly? IncidentDate { get; set; }

        [Required, MaxLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Range(0.01, 1_000_000_000)]
        public decimal? EstimatedAmount { get; set; }
    }
}
