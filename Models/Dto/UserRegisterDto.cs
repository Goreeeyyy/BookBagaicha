using System.ComponentModel.DataAnnotations;

namespace BookBagaicha.Models.Dto
{
    public class UserRegisterDto
    {

        [Required(ErrorMessage = "First Name is required")]
        public string? FirstName { get; set; }

        [Required]
        public string? LastName { get; set; }

        [Required]
        public string? Address { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Your Email is required")]
        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; }

        [Compare(nameof(Password), ErrorMessage = "Comfirm Password doesn't match Password")]
        public string? ConfirmPassword { get; set; }
        public string? Role { get; set; }
    }
}
