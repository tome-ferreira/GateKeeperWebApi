namespace GateKeeperWebApiV1.Objects
{
    public class BuildingDto
    {
        public string id { get; set; }
        public string name { get; set; }
        public string? description { get; set; }


        public BuildingDto(string Id, string Name, string?  Description)
        {
            id = Id;
            name = Name;
            description = Description;
        }
    }

    
}
