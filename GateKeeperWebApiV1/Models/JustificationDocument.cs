namespace GateKeeperWebApiV1.Models
{
    public class JustificationDocument
    {
        public Guid Id { get; set; }

        // Foreign key to the associated justification
        public Guid AbsenceJustificationId { get; set; }

        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public DateTime UploadDate { get; set; }

        // Navigation property
        public AbsenceJustification AbsenceJustification { get; set; }  // Each document belongs to one justification
    }
}