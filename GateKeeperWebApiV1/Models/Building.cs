namespace GateKeeperWebApiV1.Models
{
    public class Building
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;

        // Foreign key to the Company
        public Guid CompanyId { get; set; }

        // Navigation property to reference the Company
        public Company Company { get; set; }
        public ICollection<Movement> Movements { get; set; } = new List<Movement>();
        public ICollection<Shift> Shifts { get; set; } = new List<Shift>();

        public Building()
        {
            
        }

        public Building(string name, string? description, Guid companyId)
        {   
            Name = name;
            Description = description;
            CompanyId = companyId;
        }
    }
}
