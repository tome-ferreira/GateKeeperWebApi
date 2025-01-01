namespace GateKeeperWebApiV1.Models
{
    public class Movement
    {
        public Guid Id { get; set; }
        public Guid WorkerId { get; set; }
        public Guid? ShiftId { get; set; } 
        public DateTime DateTime { get; set; }
        public bool isEntrance { get; set; }

        // Navigation properties
        public WorkerProfile Worker { get; set; }   // A movement belongs to one worker
        public Shift? Shift { get; set; }           // A movement can have 1 or 0 shifts
    }
}
