using System.ComponentModel.DataAnnotations;

namespace OlnlineBanking.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Login { get; set; }
        public string Address { get; set; }
        [Required]
        public bool IsActivated { get; set; }
        [Required]
        public bool IsBlocked { get; set; }
    }
}