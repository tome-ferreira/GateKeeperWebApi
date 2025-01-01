using GateKeeperWebApiV1.Models;

namespace GateKeeperWebApiV1.Objects
{
    public class LoginResponse
    {
        public string companyName { get; set; }
        public string token { get; set; }
        public BuildingDto[] buildings { get; set; }
    }
}
