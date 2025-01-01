namespace GateKeeperWebApiV1.Models
{
    public class ShiftDaysOfWeek
    {
        public Guid Id { get; set; }
        public int DayOfWeek { get; set; }
        public bool isOvernight { get; set; }


        // Foreign Key
        public Guid ShiftId { get; set; }

        // Navigation property
        public Shift Shift { get; set; }    // Each ShiftDay belongs to one Shift
    }
}
