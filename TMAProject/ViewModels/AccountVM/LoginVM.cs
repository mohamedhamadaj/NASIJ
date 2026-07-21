using System.ComponentModel.DataAnnotations;

namespace TMAProject.ViewModels.AccountVM
{
    public class LoginVM
    {

        [Required]
        public string UserNameOrEmail { get; set; } = string.Empty;
        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
    }
}
