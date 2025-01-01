using Microsoft.AspNetCore.Identity;

namespace GateKeeperWebApiV1.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name {  get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;

        //Navigation properties
        public ICollection<EnterpirseRequest> EnterpirseRequests { get; set; }
        public ICollection<WorkerProfile> WorkerProfiles { get; set; }   
    }
}
