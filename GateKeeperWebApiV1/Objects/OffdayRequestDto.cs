namespace GateKeeperWebApiV1.Objects
{
    public class OffdayRequestDto
    {
        public string id { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public string? notes { get; set; }
        public string status { get; set; }
    }
}
