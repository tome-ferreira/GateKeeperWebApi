
using System.ComponentModel.DataAnnotations.Schema;

namespace GateKeeperWebApiV1.Models
{
    public class EnterpirseRequest
    {
        public Guid Id { get; set; }
        // Foreign key to User
        [ForeignKey("User")]
        public string UserId { get; set; }

        // Navigation property
        public ApplicationUser User { get; set; }
        //----------------------------------------------
        public string Name { get; set; } = string.Empty; 
        public string Description { get; set; } = string.Empty; 
        public string Password { get; set; } = string.Empty; 
        public string Salt {  get; set; } = string.Empty;
        public int BuildingsN { get; set; } ///
        public bool BuildingsNUnlimited { get; set; } ///

        public int RegistsPerMonth { get; set; } ///
        public bool RegistsPerMonthUnlimited { get; set; } ///

        public int Workers { get; set; } ///
        public bool WorkersUnlimited { get; set; } ///

        public int DashboardAccounts { get; set; } ///
        public bool DashboardAccountsUnlimited { get; set; } ///

        public bool HasExcel { get; set; } ///
        public DateTime CreationDate { get; set; }


        //Bob o construtor
       


        // Parameterless constructor for EF
        public EnterpirseRequest() { }


            
    }
}
