using System.ComponentModel.DataAnnotations;

namespace GateKeeperWebApiV1.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
