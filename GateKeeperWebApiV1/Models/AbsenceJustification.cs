namespace GateKeeperWebApiV1.Models
{
    public class AbsenceJustification
    {
        public Guid Id { get; set; }

        public Guid WorkerId { get; set; }
        public Guid CompanyId { get; set; }
        public DateTime AbsenceStarted { get; set; }
        public DateTime AbsenceFinished { get; set; }
        public string Justification { get; set; } = string.Empty;
        public string Status {  get; set; } = string.Empty; 
        // Navigation properties
        public WorkerProfile Worker { get; set; }   // One-to-many: WorkerProfile -> AbsenceJustifications
        public Company Company { get; set; }        // One-to-many: Company -> AbsenceJustifications

        // A justification can have many documents
        public ICollection<JustificationDocument> JustificationDocuments { get; set; } = new List<JustificationDocument>();
    }
}
