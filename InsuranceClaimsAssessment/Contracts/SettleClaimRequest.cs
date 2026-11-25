using System.ComponentModel.DataAnnotations;

namespace InsuranceClaimsAssessment.Contracts
{
    public class SettleClaimRequest
    {
        [Required, Range(0.01, 1_000_000_000)]
        public decimal? Amount { get; set; }

        [Required, MaxLength(2000)]
        public string Reason { get; set; } = string.Empty;
    }
}
