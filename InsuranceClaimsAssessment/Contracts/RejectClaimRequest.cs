using System.ComponentModel.DataAnnotations;

namespace InsuranceClaimsAssessment.Contracts
{
    public class RejectClaimRequest
    {
        [Required, MaxLength(2000)]
        public string Reason { get; set; } = string.Empty;
    }
}
