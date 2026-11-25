namespace InsuranceClaimsAssessment.Contracts
{
    public class ClaimDetailsResponse : ClaimResponse
    {
        public required bool WaitingForClaimant { get; set; }

        public required List<ClaimHistoryItem> History { get; set; }

        public required List<InformationRequestItem> InformationRequests { get; set; }

        // Null until the claim is settled or rejected
        public required ClaimDecisionItem? Decision { get; set; }
    }

    public class ClaimHistoryItem
    {
        public required string Status { get; set; }

        // UTC
        public required DateTime ChangedAt { get; set; }
    }

    public class InformationRequestItem
    {
        public required int Id { get; set; }

        public required string Message { get; set; }

        // UTC
        public required DateTime RequestedAt { get; set; }

        public required string? ResponseText { get; set; }

        // UTC
        public required DateTime? RespondedAt { get; set; }
    }

    public class ClaimDecisionItem
    {
        public required string Outcome { get; set; }

        public required string Reason { get; set; }

        public required decimal? SettledAmount { get; set; }

        // UTC
        public required DateTime DecidedAt { get; set; }
    }
}
