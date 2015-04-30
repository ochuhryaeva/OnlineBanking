using System.ComponentModel.DataAnnotations;

namespace OlnlineBanking.Models
{
    public class UserLoginViewModel
    {
        [Required(ErrorMessage = "Please enter your login")]
        public string Login { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Please enter your password")]
        public string Password { get; set; }
        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
}