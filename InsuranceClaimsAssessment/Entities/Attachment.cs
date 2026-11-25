namespace InsuranceClaimsAssessment.Entities
{
    public class Attachment
    {
        public int Id { get; set; }

        public int ClaimId { get; set; }
        public Claim Claim { get; set; } = null!;

        // Null when the file was not sent in reply to an information request
        public int? InformationRequestId { get; set; }
        public InformationRequest? InformationRequest { get; set; }

        public required string FileName { get; set; }

        public required string ContentType { get; set; }

        public long SizeBytes { get; set; }

        public required byte[] Content { get; set; }

        // UTC
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}
