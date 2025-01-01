namespace GateKeeperWebApiV1.Objects
{
    public class AbsenceJustificationDto
    {
        public string id {  get; set; }
        public DateTime start {  get; set; }
        public DateTime end { get; set; }
        public string justification { get; set; }
        public string status { get; set; }
    }
}
