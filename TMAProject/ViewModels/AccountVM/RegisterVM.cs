using System.ComponentModel.DataAnnotations;

namespace TMAProject.ViewModels.AccountVM
{
    public class RegisterVM
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        [Required]
        public string UserName { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        public string Passwod { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Passwod))]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
