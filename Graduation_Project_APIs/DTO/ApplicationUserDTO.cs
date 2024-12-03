using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Graduation_Project_APIs.DTO
{
    public class ApplicationUserDTO
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
        [Required]
        public string Email { get; set; }
    }
}
