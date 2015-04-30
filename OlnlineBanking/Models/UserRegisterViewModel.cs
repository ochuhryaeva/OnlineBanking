using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace OlnlineBanking.Models
{
    public class UserRegisterViewModel
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }
        [Required(ErrorMessage = "Please enter your email")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(".*@.*",ErrorMessage = "Please enter correct e-mail")]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Please enter your password")]
        [StringLength(16,MinimumLength = 8,ErrorMessage = "Password length must be betwwen 8 and 16 symbols")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Please confirm your password")]
        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Passwords must be the same")]
        public string ConfirmPassword { get; set; }
        [Required(ErrorMessage = "Please enter your login")]
        public string Login { get; set; }
        public string Address { get; set; }
    }
}