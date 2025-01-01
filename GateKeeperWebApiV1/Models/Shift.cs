namespace GateKeeperWebApiV1.Models
{
    public class Shift
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public TimeOnly Starts { get; set; }
        public TimeOnly Ends { get; set; }
        public DateTime StartsDate { get; set; }
        public DateTime EndsDate { get; set; }
        public bool IsOvernight { get; set; }  // Indicates if the shift spans two days
        public Guid CompanyId { get; set; }
        public Guid ShiftLeaderId { get; set; }

        // Foreign Key
        public Guid BuildingId { get; set; }

        // Navigation properties
        public Building Building { get; set; }
        public ICollection<WorkerShift> WorkerShifts { get; set; } = new List<WorkerShift>();
        public ICollection<ShiftDays> ShiftDays { get; set; } = new List<ShiftDays>();
        public ICollection<ShiftDaysOfWeek> ShiftDaysOfWeeks { get; set; } = new List<ShiftDaysOfWeek>();
        public WorkerProfile ShiftLeader { get; set; }

        // Add this property for the relationship
        public ICollection<Movement> Movements { get; set; } = new List<Movement>();
    }
}
