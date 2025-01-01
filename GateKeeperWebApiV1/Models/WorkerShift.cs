namespace GateKeeperWebApiV1.Models
{
    public class WorkerShift
    {
        public Guid WorkerId { get; set; }
        public WorkerProfile Worker { get; set; }

        public Guid ShiftId { get; set; }
        public Shift Shift { get; set; }
    }
}
